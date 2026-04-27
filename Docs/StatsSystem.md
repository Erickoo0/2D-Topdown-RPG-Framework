# 🛠 System: Stats & Progression System

## 📝 Overview
The **Stats System** provides a highly modular, event-driven framework for managing entity vitality (Health, Mana) and progression (Level, Experience). 

By strictly separating the mathematical logic (`Health`, `Level`) from the visual representation (`HealthUI`, `LevelUI`, `ProgressBar`), this system ensures that entities can take damage, heal over time, or gain experience without any hard dependencies on the user interface. It also includes designer-friendly tools, such as the ability to override standard mathematical EXP formulas with custom Unity `AnimationCurve` graphs.

---

## 🛑 The Challenge (The "Problem")
A common approach when building stats is to tightly couple the player's health or level directly into the main player controller, and then have a UI script constantly check those values inside an `Update()` loop. 

- **Complexity:** Hardcoding progression formulas makes it difficult for designers to balance the game. If a designer wants a massive EXP spike at level 50, standard math formulas make this rigid and tedious.
- **Dependencies:** The core health logic should not crash if a UI element is destroyed or missing. Furthermore, the UI shouldn't care if it's reading health from the Player, a Boss, or a destructible crate.
- **Scalability:** If a developer wants to add Mana, Stamina, or Energy, a poorly designed system requires rewriting the entire UI logic for every single new stat. 

---

## 🏗 The Architecture (The "Solution")
The system utilizes decoupled components that communicate strictly through events, allowing for highly reusable generic scripts (like `ProgressBar`) that can be driven by any stat type.

### 🧩 Patterns & Principles Used:
* **Observer Pattern:** Extensive use of C# `event Action` (e.g., `OnHpUpdated`, `OnLevelUpdated`) ensures the UI only updates precisely when a value changes, eliminating expensive polling in `Update()`.
* **Single Responsibility Principle:** `Health` solely manages vitality math. `ProgressBar` solely manages UI fills and text. `HealthUI` acts as the bridge connecting the two. 
* **Designer-Centric Design (Strategy Pattern approach):** The `Level` system allows designers to toggle between a standard polynomial math formula ($\text{Required EXP} = \text{Base} \times \text{Level}^{\text{Scaling Factor}}$) or a custom visual `AnimationCurve` for precise, bespoke progression balancing.
* **Component-Based Architecture:** `Health` and `Mana` are built as identical, standalone components. They can be attached to any `GameObject` to instantly grant it vitality or resources, completely independent of what the entity actually is.

---

## 💻 Code Highlights

### Smart Property Setters & Event Invocation
Instead of having scattered logic for applying damage or healing, the `Health` component relies on a highly controlled C# Property. This ensures that health is always clamped safely, floating combat text is cleanly requested via the `EventBus`, and listeners are only notified if the value *actually* changes. 

```csharp
public float HpCurrent
{
    get => _hpCurrent;
    set
    {
        float hpPrevious = _hpCurrent;

        // Clamp health so it never goes below 0 or above max.
        _hpCurrent = Mathf.Clamp(value, 0, hpMax);

        // Only notify listeners if health actually changed.
        if (!Mathf.Approximately(_hpCurrent, hpPrevious))
        {
            float difference = _hpCurrent - hpPrevious;
            int differenceRounded = Mathf.RoundToInt(difference);
            
            // Decoupled communication to global UI
            EventBus.RequestFloatingText(differenceRounded, transform.position);
            
            // Notify local components (like HealthUI)
            OnHpUpdated?.Invoke();
        }

        // If health hit zero
        if (_hpCurrent <= 0 && !_isDead) SetDead();
    }
}
```

### Robust Multi-Leveling Logic

Using a while loop ensures the entity correctly cascades through multiple level-ups, subtracting the specific required EXP for each distinct level tier, until the remaining EXP stabilizes.

```csharp
public void AddExperience(int amount)
{
    if (LvlCurrent >= lvlMax || amount < 0) return;
    
    expCurrent += amount;
    
    // Handle potential multiple level ups from a single large exp gain
    while (expCurrent >= ExpToNextLvl && lvlCurrent < lvlMax)
    {
        expCurrent -= ExpToNextLvl;
        lvlCurrent += 1;
        
        OnLevelUpdated?.Invoke();

        // Cap out safely if max level is hit during the loop
        if (lvlCurrent >= lvlMax)
        {
            expCurrent = 0;
            OnMaxLevelReached?.Invoke();
            break;
        }
    }
    
    OnExperienceGained?.Invoke();
}
```
