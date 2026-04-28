# 🛠 System: World Time & Lighting System

## 📝 Overview

The **World Time System** is a global time simulation engine that tracks the passage of time, manages a custom seasonal calendar, and drives dynamic environment lighting.

The system translates real-world frame time into game-world minutes, hours, days, and years. It is built to be highly modular: `WorldTime` acts as the ticking engine, `WorldTimeCalendar` translates raw time into a date format (Spring, Summer, etc.), `WorldTimeUI` handles the clock and calendar display, and `WorldLight` uses the time data to drive a 2D global light gradient for day lighting

Currently, the system supports configurable day lengths, editor-time lighting previews via `[ExecuteAlways]`, and a custom month/year cycle that deviates from the standard 365-day calendar.

---

## 🛑 The Challenge (The "Problem")

Implementing a time system purely through simple counters in a `Update()` loop often leads to several issues:

- **Floating Point Drift:** Simply adding `Time.deltaTime` to a "minutes" variable can cause precision loss over long play sessions, leading to the clock "stuttering" or losing time.
- **Tightly Coupled Visuals:** If the time script directly changes the light color and the UI text, adding a second light source or a different UI layout requires modifying the core time logic.
- **Editor Workflow:** Designers need to see what "Midnight" or "Noon" looks like without pressing Play every time they adjust a color gradient.
- **Non-Standard Calendars:** Most games use custom month lengths (e.g., a 10-day month). Standard C# `DateTime` objects are too rigid for this, requiring a custom math-based calendar.

---

## 🏗 The Architecture (The "Solution")

The system utilizes a "Single Source of Truth" pattern mediated by an **Event Bus**:

* **`WorldTime`**: The core engine. It uses a `minuteAccumulator` to ensure frame-rate independent time passage and broadcasts the current `TimeSpan` via the `EventBus`.
* **`WorldTimeCalendar`**: A logic layer that consumes raw time and converts it into game-specific dates using `WorldTimeConstants`.
* **`WorldLight`**: A visual controller that maps the "Percent of Day" to a `UnityEngine.Gradient`. It uses `[ExecuteAlways]` to allow visual debugging in the Scene view.
* **`WorldTimeConstants`**: A static configuration file that ensures the Calendar and the Lighting system use the same math for "How long is a day?"

### 🧩 Patterns & Principles Used:

* **Observer Pattern (EventBus):** The `WorldTime` script doesn't know the UI or Light system exists. It simply shouts "The time is now X," and interested systems react.
* **Separation of Concerns:** Time calculation (Logic), Date formatting (Data), and Light manipulation (Visuals) are all in separate classes.
- **Accumulator Pattern:** Using a `_minuteAccumulator` ensures that even if a frame takes a long time, the "leftover" seconds are saved for the next minute calculation, preventing time drift.
* **Editor Tooling:** `OnValidate` and `[ExecuteAlways]` hooks allow the Lighting system to update in real-time as designers slide values in the Inspector.

---

## 💻 Code Highlight

This snippet from `WorldTime` shows the core logic for translating real-world seconds into game minutes. It uses a ratio based on the desired `dayLength` and employs an accumulator to maintain high precision.

```csharp
private void Update()
{
    // 1. Calculate how many real-world seconds passed this frame
    float secondsPassedThisFrame = Time.deltaTime;

    // 2. Convert real seconds into game minutes using the configured ratio
    // Ratio: (Total Minutes in a Day / Real seconds for a full Day cycle)
    float gameMinutesPassed = secondsPassedThisFrame * (WorldTimeConstants.MinutesInDay / dayLength);

    _minuteAccumulator += gameMinutesPassed;

    // 3. Precision Check: Only update the global clock when a full minute is reached
    if (_minuteAccumulator >= 1f)
    {
        int minutesToAdd = Mathf.FloorToInt(_minuteAccumulator);
        _minuteAccumulator -= minutesToAdd; // Keep the remainder for the next frame

        _currentTime += TimeSpan.FromMinutes(minutesToAdd);
        
        // 4. Broadcast the new time to the entire project
        EventBus.RequestUpdateWorldTime(this, _currentTime);
    }
}
```

This second highlight from WorldLight demonstrates how the environment reacts to the time. By calculating the "Percent of Day," 
we can evaluate a Gradient to get the exact color for any time (e.g., 0.5 is Noon, 0.0/1.0 is Midnight).

```csharp
private void UpdateWorldLight(object sender, TimeSpan time)
{
    // Evaluate the color gradient based on how far we are through the current day
    _light2D.color = gradient.Evaluate(PercentOfDay(time));
}

private float PercentOfDay(TimeSpan time)
{
    // Use modulo to ignore total days/years passed and only get the current 24h progress
    return (float)time.TotalMinutes % WorldTimeConstants.MinutesInDay / WorldTimeConstants.MinutesInDay;
}
```
