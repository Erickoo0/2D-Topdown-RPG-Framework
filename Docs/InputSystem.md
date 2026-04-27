# 🛠 System: Input System

## 📝 Overview

The **Input System** handles player-facing controls across both gameplay and UI, including movement, dashing, interaction, active item usage, pause menus, and map/menu toggles.

Instead of hardcoding key checks throughout different scripts, the framework uses Unity’s modern `InputAction.CallbackContext` workflow. Input methods receive player intent, then forward that intent to the correct system, such as the player state machine, interaction detector, equipment system, or menu event flow.

This keeps input logic centralized, flexible, and easier to expand for future features like controller support, rebinding, dialogue navigation, or separate gameplay/UI action maps.

---

## 🛑 The Challenge (The "Problem")

A simple prototype might check input directly in multiple `Update()` methods using hardcoded keys. That works early on, but quickly becomes messy once the game has movement, inventory, pause menus, item usage, dialogue, and world interaction all competing for controls.

- **Complexity:** If every system checks its own keys, input behavior becomes scattered and hard to debug. Multiple systems may respond to the same button press, especially when gameplay and menus overlap.
- **Dependencies:** Movement, interaction, inventory, equipment, and UI should not all directly depend on each other. Pressing a button should communicate intent, while each system handles its own response.
- **Scalability:** As new actions are added, such as dialogue choices, hotbar shortcuts, abilities, or controller input, hardcoded checks would require constant rewrites. A callback-based input structure makes new actions easier to add without breaking existing systems.

---

## 🏗 The Architecture (The "Solution")

The system uses Unity’s Input System to route actions into focused callback methods. Each callback is responsible for a small piece of player intent rather than full gameplay behavior.

For example, movement input is read by the `PlayerController`, but actual movement is handled through the player state machine and `EntityMover`. Interaction input is received by the interaction detector, but the target object decides what interaction means. Menu input requests UI changes through the event system instead of directly controlling all menu logic.

### 🧩 Patterns & Principles Used:

* **Command-Style Input Actions:** Input actions represent player intent, such as move, dash, interact, or open menu, instead of being tied to hardcoded keys.
* **Single Responsibility Principle:** Input callbacks route intent but avoid owning entire gameplay systems.
* **State Machine Integration:** Movement and dash input are exposed to the player state machine, allowing states to decide how to respond.
* **Interface-Driven Interaction:** The interact input calls `IInteractable.Interact()` without needing to know whether the target is an NPC, item, chest, or quest object.
* **Event-Driven UI Flow:** Pause and map inputs request menu open/close behavior through the event bus, keeping UI management decoupled from raw input.
  
---

## 💻 Code Highlight
This method is a strong example of context-sensitive input design. The same button can interact with the world when a valid target exists, or fall back to using the active equipped item when there is no target.
```csharp
public void OnInteract(InputAction.CallbackContext context)
{
  if (!context.performed) return;
  // 1. Check and Trigger Interaction
  if (_interactableTarget != null && _interactableTarget.CanInteract())
  {
      PlayerController playerController = GetComponent<PlayerController>();
      _interactableTarget.Interact(playerController);
  }
  
  // 2. Item Use
  else if (PlayerEquipment.Instance != null)
  {
      PlayerEquipment.Instance.TryUseActiveItem();
  }
}
```

