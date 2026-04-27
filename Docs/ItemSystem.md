# 🛠 System: Item System

## 📝 Overview

The **Item System** defines how items are created, stored, displayed, dropped, picked up, and used throughout the framework.

The system is built around a core structure backed by a centralized registry: `ItemDataSo` stores the static item definition, `ItemInstance` stores runtime item state such as stack size, and `ItemObject` represents the physical item in the world. Tying this all together is the **ItemDatabase**, a global registry that maps unique string IDs to their respective `ItemDataSo`. 

This separation allows the same item data to be reused across inventory slots, world drops, containers, consumables, and future equipment or quest items, while making saving and loading lightweight and reliable. Currently, the system supports stackable items, item icons, item values, world pickups, animated item drops, and usable consumables.

---

## 🛑 The Challenge (The "Problem")

A common beginner approach is to make every item a unique prefab with all of its data and behavior directly attached to the GameObject, or to serialize entire ScriptableObjects into save files. This works for a small scope, but becomes difficult once items need to exist in multiple forms or persist between game sessions.

- **Complexity:** If item data, inventory state, and world physics are all stored in one object, the system becomes hard to reason about and easy to break.
- **Dependencies:** The inventory should not depend on a world pickup object, and a world pickup should not be the source of truth for item stats, icons, value, or use behavior.
- **Scalability:** Adding new item types like weapons, potions, or materials should not require rewriting pickup and UI logic every time.
- **Serialization & Data Persistence:** Saving the entire state of an item object is bloated. The game needs a way to save just a simple string like `"potion_health_01"` and perfectly reconstruct the item upon loading.

---

## 🏗 The Architecture (The "Solution")

The system separates items into clear, distinct layers:

* **`ItemDataSo`**: A ScriptableObject that stores designer-friendly item data such as ID, name, description, icon, value, stackability, and world prefab.
* **`ItemDatabase`**: Acts as the central source of truth, converting a designer-friendly List of `ItemDataSo` assets into a highly efficient runtime Dictionary. This allows systems (like the Save/Load system) to fetch any item definition instantly using its unique String ID.
* **`ItemInstance`**: Wraps an `ItemDataSo` and stores runtime data, such as the current stack size. This lets the same item definition exist in multiple independent stacks.
* **`ItemObject`**: The in-world pickup representation. It handles sprite display and physics, passing the item instance into the inventory system upon interaction.

### 🧩 Patterns & Principles Used:

* **Registry Pattern & Lazy Initialization:** `ItemDatabase` centralizes data lookups and uses a Dictionary for fast $O(1)$ retrieval, automatically initializing itself if a system requests an item before explicit setup.
* **ScriptableObject Data Architecture:** Item definitions are stored as reusable assets, making item creation designer-friendly and avoiding hardcoded values.
* **Separation of Concerns:** Static data, runtime state, and world behavior are split into separate classes.
* **Open/Closed Principle:** New item types can extend `ItemDataSo` without changing the base item pipeline.
* **Single Responsibility Principle:** `ItemDataSo` defines what an item is, `ItemInstance` tracks the owned copy, and `ItemObject` handles world pickup behavior.

---

## 💻 Code Highlights

### Defensive Database Initialization
To make the database both designer-friendly (using a visible List in the inspector) and performant (using a Dictionary at runtime), the system converts the data upon initialization. 

A strong design choice here is the **defensive programming**. By utilizing `TryAdd` and checking for nulls, the database gracefully handles common designer errors—like forgetting to assign an item or accidentally duplicating an ID—without throwing game-breaking exceptions.

```csharp
public void Initialize()
{
    // Create a fresh dictionary for O(1) lookups
    _itemDictionary = new Dictionary<string, ItemDataSo>();

    foreach (ItemDataSo itemData in allItems)
    {
        if (itemData == null)
        {
            Debug.LogWarning("[ItemDatabase] ItemData is null!");
            continue;
        }
        
        // Pairs itemID to itemData, returns warning if itemID is already used
        // This prevents the application from crashing due to duplicate keys
        if (!_itemDictionary.TryAdd(itemData.ItemID, itemData))
        {
            Debug.LogWarning($"[ItemDatabase] Duplicate Item ID found: {itemData.ItemID}. IDs must be unique!");
        }
    }
}
```

### Separating State from Representation

The ItemObject does not define what the item is from scratch; it receives an ItemInstance and updates its physical representation accordingly.

Using a nullable Vector3? dropTarget allows the same method to support both manually placed items and animated drops from containers or enemies, keeping the API flexible

```csharp
public void SetItemObject(ItemInstance newItemInstance, Vector3? dropTarget = null, bool animate = true)
{
    _itemInstance = newItemInstance;
    // Renaming the GameObject in the hierarchy helps with debugging runtime spawns
    gameObject.name = _itemInstance.DataSo.ItemName;
    
    // Logic for animating to dropTarget would follow...
}
```
