# 📦 Technical Deep Dive: Inventory System

## **Architecture Overview**
The inventory system is built on a **Decoupled Data-Driven Architecture**. By separating the data (Logic) from the representation (UI), the system is highly scalable, performance-optimized, 
avoids the "spaghetti code", and is extremely modular. 

---

## **1. The Data Model (The "Four-Tier" System)**
To optimize memory and ensure clean serialization, items are split into four distinct classes:

* **ItemData (ScriptableObject):** The "Blueprint." Contains immutable data like icons, names, descriptions, and prefabs.
* **ItemInstance (C# Class):** The "Individual Item." Tracks dynamic state such as `stackSize` and `durability`. 
* **ItemObject (MonoBehaviour):** The "Physical World Item." Handles item pickup, physics, and effects for items on the ground.
* **ItemDatabase (ScriptableObject):** The "Map." Maps unique IDs to `ItemData` for efficient JSON saving and loading.

---

## **2. InventoryManager (Single Source of Truth)**
The `InventoryManager` is a Singleton that acts as the central authority for all inventory logic and holds all Inventory data. 

### **Key Responsibilities:**
1.  **State Management:** Holds the master `ItemInstance[]` array.
2.  **Logic Operations:** Handles the math for stacking logic, item swapping in inventory, adding and dropping items.
3.  **Observer Pattern:** Uses **C# Actions** (`OnSlotUpdated`) to notify listeners that a slot/item has been updated without needing hard references to the UI.

---

## **3. InventoryUI & SlotUI (The Passive View)**
Following the **Passive View pattern**, the IventoryUI & SlotUI holds no item data and is strictly for visualization.

### **InventoryUI**
* **Dynamic Generation:** Programmatically spawns `SlotUI` prefabs based on the `InventoryManager`'s size.
* **Targeted Refresh:** Listens for update events and only refreshes the specific slot index that changed, rather than rebuilding the entire grid.

### **SlotUI**
* **Zero Data Ownership:** The slot does not "hold" an item. It holds a `slotIndex` and pulls data from the InventoryManager only when needed.
* **Interface Implementation & Polymorphism:** Implements `IStorageSlot`, allowing external systems (like the DragManager) to interact with any slot type polymorphically.

---

## **4. S.O.L.I.D. Principles Applied**
* **Single Responsibility:** Logic (Manager), Display (UI), and Data (ScriptableObjects) are strictly separated.
* **Interface Segregation:** Systems like the `DragManager` depend on the `IStorageSlot` interface rather than a concrete `SlotUI` class.
* **Dependency Inversion:** High-level UI logic depends on abstractions (Interfaces/Events), making it easy to add Chests or Vendors later.

---

## **Future Expandability**
Because the system is decoupled via the `IStorageSlot` interface, the system is ready for:
* **Equipment Systems** (using the same slot logic but filtered by item type).
* **Chest/Storage Systems** (pointing the UI to a different `ItemInstance` array).
* **Sorting Algorithms** (manipulating the array in the Manager without breaking the UI).
