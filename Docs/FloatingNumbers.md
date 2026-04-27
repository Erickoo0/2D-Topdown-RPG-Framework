# Floating Combat Text System Technical Deep Dive

A highly performant, globally accessible floating number system built in **Unity / C#** with a focus on **loose coupling**, **zero-allocation object pooling**, and **2D pixel-perfect rendering**.

---

## Overview

The floating text system is built around a **Silent Listener / Event Bus architecture** to eliminate hard dependencies between gameplay and visual feedback.

Key design ideas:
- **`CombatEvents`** acts as a global message broker for transient feedback
- **`FloatingNumberManager`** commands the object pool as a completely decoupled background service
- **`FloatingText`** handles its own animation lifecycle and returns itself to the pool
- **Custom Queue-based Pooling** eliminates runtime garbage collection (GC) spikes
- **World-Space TextMeshPro** completely bypasses expensive UI Canvas rebuilds
- **DOTween** is leveraged for optimized, code-driven animations

This keeps core gameplay systems (like health and combat) entirely unaware of the UI/feedback layer, ensuring maximum extensibility.

---

## Architecture and Event Flow

The system strictly separates **state tracking** from **transient feedback** by utilizing a Domain-Specific Event Bus (`CombatEvents.cs`).

- **Game Entities** (Player, Enemies, Traps) do not reference the UI. When damage or healing occurs, they simply invoke a static `Action` on the event bus, passing only the value and their `Vector3` world position.
- **`FloatingNumberManager`** quietly subscribes to this bus. It listens for requests, spawns the text, and handles the visual representation.

This **fire-and-forget** approach prevents the "spiderweb dependency" problem. Dynamically spawned enemies or newly added hazards can trigger floating numbers immediately without needing to register themselves with a central UI manager. If the `FloatingNumberManager` is removed from the scene, the game continues to run without throwing null reference exceptions.

---

## Object Pooling and Memory Management

To maintain a steady framerate, the system explicitly avoids using `Instantiate()` and `Destroy()` during combat. 

`FloatingNumberManager` implements a custom **FIFO (First In, First Out)** Object Pool using a standard C# `Queue<FloatingText>`:
- **Pre-warming:** A set number of text objects are instantiated and deactivated during `Awake()`.
- **Retrieval:** When an event fires, the manager `Dequeues` an available text object, updates its state, and activates it.
- **Recycling:** Once the animation concludes, the `FloatingText` object utilizes an `Action` callback to inform the manager it is done. It is deactivated and `Enqueued` back into the pool.

This ensures zero memory allocation during gameplay, preventing micro-stutters caused by Unity's Garbage Collector during combat-heavy moments.

---

## Rendering and Animation (2D Pixel Art)

Instead of relying on Unity's Screen Space UI Canvas, the system utilizes **World-Space TextMeshPro**. 

Because Canvases must recalculate their layout whenever a child element changes or moves, updating 10 floating numbers every frame on a main Canvas creates significant overhead. World-Space TMP acts identically to a standard `SpriteRenderer`, existing natively in the 2D coordinate space. 

To maintain a **crisp pixel art aesthetic**:
- The TMP Font Asset is generated using a **Raster** render mode (bypassing SDF smoothing).
- The font atlas texture is set to **Point (no filter)**.
- The component utilizes built-in **Sorting Layers** to render correctly above sprites and environments.

Animation is handled via **DOTween**. Rather than relying on verbose and potentially expensive Unity Coroutines, the `FloatingText` script uses optimized, chained tweens to handle vertical translation and alpha fading, firing an `OnComplete` callback to seamlessly return itself to the queue.

---

## Summary

This floating number system is built around:
- **Total decoupling** of gameplay and visual feedback via a Domain Event Bus
- **Zero-allocation performance** through a custom Queue-based Object Pool
- **Bypassing UI Canvas overhead** by using World-Space TextMeshPro
- **Crisp 2D rendering** tailored specifically for pixel-perfect aesthetics
- **Self-managing text instances** that handle their own tweening and recycling callbacks

The result is a lightweight, infinitely scalable feedback system that can be dropped into any scene without requiring setup or hard references from existing combat scripts.
