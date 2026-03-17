# Inventory System Technical Deep Dive

A modular inventory system built in **Unity / C#** with a focus on **clean architecture**, **event-driven updates**, and **extensibility**.

---

## Overview

The inventory is built around a **single source of truth**: `InventoryManager`.

Key design ideas:
- **`ItemData`** defines static item info using `ScriptableObject`
- **`ItemInstance`** stores runtime item state such as stack count
- **`InventoryManager`** owns all inventory contents and logic
- **UI updates react to events** instead of polling every frame
- **The hotbar is the first section of the inventory**
- **Save/load uses item IDs**, not direct object references

This keeps the system easy to reason about while leaving room for future features.

---

## Data Model and Item Flow

The inventory separates **static item data** from **dynamic item data**:

- **`ItemData`** is the shared blueprint for an item type  
  Includes ID, icon, name, description, prefab reference, and booleans

- **`ItemInstance`** is the live item stack
  Holds a reference to `ItemData` and holds dynamic information like durability, or stack size

This avoids duplicating static data across slots and creates a flexible base for future per-item properties.

The same model is also used across the full item lifecycle:
- **world pickups** are represented by `ItemObject`
- **inventory slots** store `ItemInstance`
- **save data** stores item ID, slot index, and stack size
- **loaded items** are rebuilt through `ItemDatabase`

Using IDs for persistence keeps save data lightweight and avoids relying on direct scene or prefab references.

---

## Inventory, Hotbar, and Equipment

`InventoryManager` stores the inventory as a fixed-size array of `ItemInstance` and handles:
- adding and stacking items
- removing items
- swapping slots
- dropping items
- notifying other systems when data changes

The hotbar is not a separate inventory. It is simply the **first section of the same slot array**.

This keeps the architecture simpler by avoiding duplicated logic between inventory and hotbar systems.

`HotbarManager` listens for Hotbar related input and handles active item selection , while `PlayerEquipment` listens for active slot changes and spawns the selected item prefab as the equipped object. If that object implements `IUsable`, it can perform item-specific behavior when used.

This keeps item usage **component-driven** and avoids hardcoding item behavior into the inventory manager.

---

## UI and Event-Driven Updates

The UI is built as a **reactive presentation layer**.

`InventoryUI` creates the slot UI and listens for inventory events. Each slot:
- knows its index
- reads data directly from `InventoryManager`
- refreshes only when its slot changes through events sent from `InventoryManager`

This event-driven approach improves:
- separation of concerns
- decoupling between gameplay and UI
- maintainability as the system grows

Rather than treating the UI as a second owner of inventory data, it stays synchronized by responding to changes from the manager.

---

## Summary

This inventory system is built around:
- **centralized state management** through a single inventory manager
- **separation of static and runtime item data** using `ItemData` and `ItemInstance`
- **event-driven updates** between inventory, UI, and equipment systems
- **a unified inventory and hotbar model** using one shared slot array
- **ScriptableObject-based item definitions** for clean content authoring
- **ID-based save/load flow** for stable persistence
- **extensible item behavior** through interfaces and runtime instances

The result is a compact system with clear ownership, reactive UI updates, and a consistent flow between world items, inventory storage, and player interaction.
