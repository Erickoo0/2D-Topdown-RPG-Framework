# 🛠 System Deep Dive: Modular Quest Framework

## 📝 Overview
The **Quest System** is a highly modular, event-driven, and data-driven framework designed to handle the complete lifecycle of a player's journey. From quest acceptance and tracking to UI presentation and JSON serialization, the system is built to scale gracefully. 

By leveraging an `EventBus` for global communication and a strict separation of concerns between static data, runtime state, and UI, this system allows designers to build complex, multi-objective quests in the Unity Inspector without writing a single line of code.

---

## 🛑 The Challenge: Avoiding the "God Class"
Quest systems are notorious for turning into monolithic "God Classes." When built poorly, they create hard dependencies across the entire codebase. This architecture specifically targets four common pitfalls:

* **The Dependency Web:** Usually, an enemy's `OnDeath` method calls `QuestManager.Instance.AddKill()`. This couples combat to quests. Here, systems remain "blind" to each other, communicating only through anonymous broadcasts.
* **The ScriptableObject Mutation Trap:** Beginners often modify `ScriptableObject` fields at runtime (e.g., `questData.currentKills++`). This permanently overwrites the asset on the hard drive. This system treats SOs as **Immutable Blueprints**.
* **UI Entanglement:** Mixing backend logic with UI code (e.g., updating a Text component inside a progress loop) makes the system rigid. This framework separates the "Live Data" from the "Visual Representation."
* **Save File Bloat:** Attempting to serialize entire complex quest objects leads to circular reference errors. By separating the **Blueprint** from the **Progress**, we only save a small array of integers per quest.

---

## 🏗 The Architecture: A Three-Tier Approach

### 🧩 1. The Blueprint Layer (Static & Immutable)
* **`QuestSo` (ScriptableObject):** The source of truth. It holds read-only metadata: ID, Name, Description, and an array of required objectives.
* **`QuestObjective` (ScriptableObject):** A modular definition of a task (e.g., "Kill 10 Slimes"). Because these are individual assets, designers can mix and match them into a `QuestSo` via the Inspector.

### ⚙️ 2. The Runtime Layer (Dynamic State)
* **`QuestActive` (Standard C# Class):** The **Wrapper Pattern** in action. This class acts as a live, serializable container. It holds a reference to the static `QuestSo` but tracks `int[] ObjectiveProgress` locally. 
* **`QuestManager` (The Orchestrator):** The central authority. It maintains the master database and the player's active list. it listens to the `EventBus` to process updates and packages data for serialization.

### 🖥️ 3. The Presentation Layer (Visuals)
* **`QuestUI`:** The manager of the visual list. It listens for backend changes and handles the instantiation of quest entries.
* **`Quest.cs`:** The UI component attached to a single quest prefab. It maps data from a `QuestActive` instance directly to `TextMeshPro` fields.

---

## 🔄 System Flow: The Lifecycle of a Quest

1.  **Acceptance:** An NPC fires an event: `EventBus.Publish(new QuestRequestEvent("quest_01"))`. The `QuestManager` catches this, fetches the blueprint from the database, wraps it in a new `QuestActive` object, and notifies the UI.
2.  **Progression:** The player kills a goblin. The goblin script fires `EventBus.Publish(new ObjectiveUpdateEvent("mob_goblin", 1))`. The goblin doesn't know if a quest exists; it just "yells" into the bus. 
3.  **Validation:** The `QuestManager` hears the yell, finds the relevant `QuestActive` instance, and updates its internal progress array. 
4.  **Completion:** `QuestActive` evaluates its progress against the blueprint's requirements. Once met, it flags itself as complete, triggering reward or turn-in logic.

---

## 💻 Code Highlight 
### The Wrapper Pattern
To protect the `ScriptableObject` and facilitate a lightweight Save/Load system, `QuestActive` uses **Constructor Overloading**. This allows the system to generate a fresh quest or reconstruct an existing one from a save file with safety checks.

```csharp
[System.Serializable]
public class QuestActive
{
    public QuestSo QuestData { get; private set; }
    public int[] ObjectiveProgress { get; private set; } 
    public bool IsCompleted { get; private set; }

    // Constructor 1: Initializing a BRAND NEW quest
    public QuestActive(QuestSo questData)
    {
        QuestData = questData;
        // Dynamically size the tracking array based on the SO's requirements
        ObjectiveProgress = new int[QuestData.QuestObjectives.Length];
        IsCompleted = false;
    }
    
    // Constructor 2: Rebuilding a quest from Save Data (JSON)
    public QuestActive(QuestSo questData, int[] savedProgress, bool isCompleted)
    {
        QuestData = questData;
        ObjectiveProgress = new int[QuestData.QuestObjectives.Length];

        if (savedProgress != null)
        {
            // Safety Check: Protect against index out of bounds if the 
            // QuestSO was modified after the player's save was created.
            int count = Mathf.Min(savedProgress.Length, ObjectiveProgress.Length);
            Array.Copy(savedProgress, ObjectiveProgress, count);
        }
        IsCompleted = isCompleted;
    }
}
```
