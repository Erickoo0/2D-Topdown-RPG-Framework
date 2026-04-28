# 🛠 System: Dialogue System

## 📝 Overview

The **Dialogue System** is a node-based conversation engine that handles sequential text display, branching player choices, and event triggering (such as starting quests or opening shops).

The system is built using a **ScriptableObject-driven architecture**, where conversations are composed of `DialogueNode` objects. This allows designers to create complex, branching paths without touching code. The system manages the flow between text lines, handles "typewriter" text effects, and ensures the UI remains synced with the underlying conversation logic.

Currently, the system supports multi-line nodes, branching choices with Unity EventSystem focus management, dynamic portraits, and a  `EventBus` for triggering game-world actions directly from a conversation.

---

## 🛑 The Challenge (The "Problem")

Managing dialogue through a single massive script or hardcoded strings leads to several technical bottlenecks:

* **Linearity vs. Branching:** Handling nested `if/else` statements for branching paths becomes unmanageable as the story grows.
* **State Management:** Keeping track of whether the UI is currently "typing," waiting for a player to click "next," or waiting for a choice requires careful state handling to prevent input bugs.
* **Coupling:** If the Dialogue Manager is directly responsible for spawning shop menus or giving quest items, it becomes a "God Object" that is difficult to test or modify.
* **Input Handling:** Ensuring that keyboard/gamepad navigation doesn't "lose" focus when dynamic buttons are spawned is a common friction point in Unity UI.

---

## 🏗 The Architecture (The "Solution")

The system separates conversation data from the logic required to display and navigate it:

* **`DialogueNode` (ScriptableObject)**: Acts as a data container for a segment of conversation, holding multiple lines of text and an array of potential player responses.
* **`DialogueManager`**: The central brain. It maintains the current state (index of the line, current speaker) and controls the flow of the conversation.
* **`DialogueOptionController`**: A dedicated UI helper that dynamically spawns choice buttons and manages their lifecycle.
* **`DialogueUI`**: A visual layer that coordinates the presentation, such as updating text fields, portraits, and checking the state of the typewriter effect.

### 🧩 Patterns & Principles Used:

* **State Management:** The manager tracks `_isWaitingChoice` and `IsTyping` states to determine how input should be interpreted (e.g., skipping text vs. advancing lines).
* **Callback Pattern:** The `DialogueOptionController` uses `System.Action` to pass the player's choice back to the manager without the controller needing to know the logic of the conversation.
* **Event-Driven Integration:** Using an `EventBus`, the system can trigger external game logic (like "OpenShop") without being tightly coupled to the Shop or Quest systems.
* **Singleton Pattern:** Provides a global access point for NPCs to trigger dialogue while ensuring only one conversation instance exists.
* **Focus Guard:** A dedicated check in `Update` ensures the `EventSystem` always maintains a selection for keyboard/gamepad users when choices are present.

---

## 💻 Code Highlight

The following snippet from `DialogueManager` illustrates how the system handles the transition from "reading text" to "making a choice." 

A key design choice here is the **separation of logic**: the Manager determines *when* options should appear, but delegates the *creation* and *focus management* of those buttons to the `OptionController`.

```csharp
private void CheckForOptions()
{
    // 1. Determine if we are at the end of the current node's text
    bool isLastLine = _currentLineIndex == _currentNode.dialogueLines.Length - 1;
    bool hasOptions = _currentNode.dialogueOptions != null && _currentNode.dialogueOptions.Length > 0;

    // 2. Transition state from "Reading" to "Waiting for Choice"
    if (isLastLine && hasOptions)
    {
        _isWaitingChoice = true;
        
        // 3. Delegate UI creation to the specialized controller
        // Pass 'OnOptionSelected' as a callback to bridge the UI back to logic
        _dialogueOptionController.CreateButtons(_currentNode.dialogueOptions, OnOptionSelected); 
        
        // 4. UX Guard: Immediately set UI focus for controller/keyboard support
        GameObject firstButton = _dialogueOptionController.GetFirstButton();
        if (firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton);
        }
    }
}
```

This second highlight showcases the `Dynamic Button` Logic within the `DialogueOptionController`. It uses a lambda expression to capture the specific `DialogueOption` data at the 
moment of creation. This allows a single button prefab to carry unique data (like the next node or a quest ID) back to the manager.

```csharp
public void CreateButtons(DialogueOption[] options, System.Action<DialogueOption> onSelected)
{
    ClearOptions();

    for (int i = 0; i < options.Length; i++)
    {
        // 1. Spawn button and set visual text
        GameObject button = Instantiate(buttonPrefab, dialogueOptionPanel.transform);
        button.GetComponentInChildren<TextMeshProUGUI>().text = options[i].optionName;
        
        // 2. Capture the loop variable to avoid closure issues
        DialogueOption currentOption = options[i];
        
        // 3. Assign the callback: When clicked, run the manager's logic with this data
        button.GetComponent<Button>().onClick.AddListener(() => onSelected(currentOption));
        _buttons.Add(button);
    }
}
```

