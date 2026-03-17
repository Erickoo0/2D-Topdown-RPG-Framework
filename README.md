# 2D Topdown RPG Framework
**Robust, Modular 2D Framework Built in Unity:** As my first major project in Unity, this project serves as a foundational template designed to be scalable, designer-friendly, modular, and flexible. Focusing on clean architecture, S.O.L.I.D principles, popular design patterns, and decoupled systems.

---

## 🎯 Project Vision
This serves as a **Core Systems Framework**. I developed this to be a reusable template where gameplay mechanics are decoupled from core logic, allowing for rapid iteration, high extensibility, and clean maintenance across multiple projects.
It will also serve as a documentation of my growth and journey as a developer, specifically focusing on these **Core Pillars.**

### Core Pillars:
* **Systems-First Architecture:** Priority is placed on modular flexibility, easy additions, and extensions rather than full on features. Every system is decoupled, allowing you to swap components without breaking the project.
* **Designer-Centric Design:** Systems are exposed via the Unity Inspector with serialized variables, allowing non-coders to tweak gameplay feel without touching the source code.
* **Professional Workflow:** Establishing good practices following Industry Standards for technical documentations, naming conventions, clean code, and GIT commits.
* **Quality Systems'** Internationally choosing to build refined and clean systems over many low quality, unfinished features.

---

## 🕹️ Preview
![Project Preview GIF](https://via.placeholder.com/800x450.png?text=Add+Your+Gameplay+GIF+Here)
*Add a GIF here showing your character moving or interacting to catch the recruiter's eye.*

---

## 🛠️ Technical Implementation
Brief highlights of how I applied advanced C#\Unity concepts to real-world game development challenges in the project.

### Architectural Highlights:
* **S.O.L.I.D. Compliance:** Ensuring every class has a single responsibility. Systems are designed to be open for extension but closed for modification.
* **Decoupled Communication:** Extensive use of Delegates, Events, and the **Observer Pattern** to ensure systems (like UI and Combat) never "know" about each other directly.
* **Interface-Driven Design:** Using interfaces to create flexible, swappable components (e.g., `IDamageable`, `IInteractable`).
* **Abstractions:** Using abstract classes and inheritance to create flexible systems. allowing for systems to have shared behaviors plus unique logic, and avoid code duplication.
* **ScriptableObject Architecture:** Leveraging ScriptableObjects for data-driven design, modular stats, and game-wide event architecture.
* **Dependency Injection:** Minimizing "Singleton-dependency" to reduce hard-coded dependencies, keeping the codebase testable and clean.

---

## 🚀 Key Systems (Technical Deep Dives)
Below are links to the internal documentation detailing the mechanics, challenges, and design patterns used for each system. These files live in the `/Doc` folder.

| System | Technical Highlight | Documentation |
| :--- | :--- | :--- |
| **Input System** | FIller text. | [View Doc](./Doc/Movement.md) |
| **Items System** | 3 Part Item System ItemData(Static Data) -> ItemInstance(Dynamic Data) -> ItemObject(Physics Data). | [View Doc](./Doc/Combat.md) |
| **Inventory System** | Uses slots class as base that holds arrays of ItemInstance. InventoryManager(Data), InventoryUI(UI), HotbarManager(UI), PlayerEquipment(Data). | [View Doc](./Doc/Inventory.md) |
| **Animation System** | Proximity-based detection using the Strategy pattern. | [View Doc](./Doc/Interactions.md) |

---

## 🔧 Workflow & Tools
* **Version Control:** Practicing clean Git history and meaningful commit messages via GitHub Desktop.
* **Unity Version:** `Unity 6.3 LTS`
* **Documentation:** Every major system includes a technical breakdown of *"The Problem," "The Pattern,"* and *"The Solution."*
* **Iteration:** Regularly refactoring old code as my understanding of C# and Unity deepens. This repo serves as a record of that improvement.
