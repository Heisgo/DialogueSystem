# DialogueSystem for Unity
A centralized dialogue system designed for Unity games that streamlines interactive narrative creation with customizable commands and branching dialogues

## Overview
This project implements a robust dialogue system where all interactions are managed centrally. Built around a singleton-based DialogueManager, the system loads and controls dialogue sequences for various characters or NPCs, allowing you to create engaging and dynamic story-driven experiences

## Features
### Centralized Management:
Organize all dialogue sequences in one place for easy maintenance and scalability.

### Interactive Dialogues:
Supports branching dialogues with player choices, enabling different narrative outcomes based on in-game decisions.

### Dynamic Text Commands:
Embed commands within dialogue text to control visual effects such as text resizing, color changes, speed adjustments, delays, and even audio cues.

### Modular & Extendable:
Easily add new dialogues or characters. The system is designed to be modular, making it straightforward to integrate additional functionality or modify existing behavior.

### Lightweight Integration:
Simple to set up in any Unity project with minimal configuration required.

## How to Install?
Just drag & drop the unitypackage file to your project.

## Usage
**Triggering Dialogues:**
Use input events or game triggers to start dialogues. For example, pressing the "E" key can initiate a dialogue sequence associated with a specific character:
```csharp
if (Input.GetKeyDown(KeyCode.E) && !DialogueEngine.uiPanel.activeInHierarchy)
    DialogueManager.Instance.StartDialogue("Echo");
````
**Interactive Options:**
Define interactive messages that offer players multiple response options. Each option can trigger callbacks that lead to different dialogue branches or actions in the game.

**Custom Commands:**
Leverage embedded commands in your dialogue texts to modify presentation dynamically. Examples include commands like /Resize:up/ for increasing text size, /SetColor:Red/ to change text color, and /Delay:2.5/ to add pauses between messages.

## Example Dialogue
An example dialogue for a character named "Echo" demonstrates multiple interactive elements and dynamic text commands. The sequence includes normal messages, text commands for visual effects, and interactive questions that validate player responses through callbacks

## Contributing
Contributions are welcome! If you have ideas for enhancements, bug fixes, or additional features, please fork the repository and submit a pull request. Alternatively, feel free to open an issue to discuss your suggestions.

## License
See the LICENSE file for details.
