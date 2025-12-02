# 🎮 LTG Final Project - 2D Action Game

A Unity-based 2D action game featuring dynamic combat, shop system, enemy AI, and persistent game progression.

---

## 📌 Table of Contents
- [Project Overview](#project-overview)
- [Features](#features)
- [Game Mechanics](#game-mechanics)
- [Installation & Setup](#installation--setup)
- [Project Structure](#project-structure)
- [Key Systems](#key-systems)
- [Controls](#controls)
- [Contributing](#contributing)
- [License](#license)
- [Support](#support)
- [Credits](#credits)

---

## 🧩Project Overview

**LTG Final Project** is a 2D action game developed in Unity that combines fast-paced combat mechanics with strategic progression systems. Players engage in battles with enemies, collect rewards, upgrade their character through a shop system, and face challenging boss encounters.

- **Engine:** Unity
- **Target Platform:** PC
- **.NET Framework:** 4.7.1
- **Repository:** [GitHub - LTG_FinalProject](https://github.com/huycq2004/LTG_FinalProject)

---

## ✨Features

### Core Gameplay
- ⚔️ **Combat System** - Attack, jump, dash, and bow mechanics
- 🦘 **Player Movement** - Smooth movement with double jump and dash (invincibility frames)
- 🎯 **Ranged Combat** - Arrow-based bow attack system
- ❤️ **Health System** - Dynamic health bar with damage calculation

### Game Systems
- 🛒 **Shop Panel** - Buy upgrades for health, damage, speed, and bow
- 💰 **Currency System** - Gold collection and management
- 🎁 **Reward System** - Item drops and progression rewards
- 💾 **Data Persistence** - PlayerPrefs-based save system
- 🤖 **Enemy AI** - Golem and Boss enemy types with unique behaviors

### UI & UX
- 🏠 **Main Menu** - Start game and exit options
- ⏸️ **Pause Menu** - Continue or return to main menu
- 💀 **Game Over Screen** - Replay or quit options
- 📊 **Stats Display** - Real-time stat tracking and upgrades

---

## ⚙️ Game Mechanics

### Player Controls
| Action | Control |
|--------|---------|
| Move Left/Right | A/D or Arrow Keys |
| Jump | W or Up Arrow |
| Double Jump | W/Up Arrow (in air) |
| Dash (with iframe) | Space |
| Attack | Left Click |
| Bow Attack | Right Click |
| Shop | E |
| Pause | ESC |

### Combat System
- **Melee Attack:** Close-range damage with attack duration and radius
- **Bow Attack:** Ranged attack with arrows that deal damage on impact
- **Dash:** Quick movement with invincibility frames (iframe) and cooldown

### Progression
- **Upgrades Available:**
  - ?? Max Health
  - ? Attack Damage
  - ?? Movement Speed
  - ?? Bow Acquisition

### Enemy Types
- **Golem:** Standard enemy with patrol and attack AI
- **Boss:** Advanced enemy with complex attack patterns

---

## 🛠️ Installation & Setup

### Prerequisites
- Unity 2021.3 LTS or higher
- .NET Framework 4.7.1
- Git (optional, for cloning the repository)

### Clone the Repository

git clone https://github.com/huycq2004/LTG_FinalProject.git
cd LTG_FinalProject


### Setup in Unity
1. Open Unity Hub
2. Click "Add" ? Select the project folder
3. Open the project with compatible Unity version
4. Allow Unity to import all assets
5. Open the **START** scene to begin
6. Press Play to test the game

### First Run
- Game initializes default player stats on first play
- PlayerPrefs stores all progression data locally
- Data persists across game sessions

---

## 📁 Project Structure


Assets/
??? Scripts/
?   ??? Data/
?   ?   ??? PlayerDataManager.cs          # Player stats & save system
?   ??? UI/
?   ?   ??? GameManager.cs                # Main menu controller
?   ?   ??? GameOverManager.cs            # Game over screen
?   ?   ??? HealthBarUI.cs                # Health bar display
?   ??? Soldier/
?   ?   ??? SoldierController.cs          # Player character control
?   ?   ??? PlayerArrow.cs                # Arrow projectile
?   ??? Shop/
?   ?   ??? ShopPanel.cs                  # Shop UI & purchase logic
?   ??? Reward/
?   ?   ??? RewardItem.cs                 # Reward item system
?   ??? Enemy/
?   ?   ??? EnemyController.cs            # Base enemy class
?   ?   ??? GolemController.cs            # Golem enemy AI
?   ?   ??? BossController.cs             # Boss enemy AI
?   ??? Manager/
?   ?   ??? CurrencyManager.cs            # Gold management
?   ??? Utilities/
?       ??? [Helper scripts]
??? Scenes/
?   ??? START.unity                       # Main menu scene
?   ??? MAIN SCENE.unity                  # Gameplay scene
??? Prefabs/
?   ??? Player/
?   ??? Enemies/
?   ??? Arrows/
?   ??? UI/
??? Assets/
    ??? Sprites/
    ??? Fonts/
    ??? Audio/


---

## ?? Key Systems

### Data Persistence System
**PlayerDataManager.cs** handles all player data using Unity's PlayerPrefs:

// Saves and loads:
- Player Gold
- Current Health & Max Health
- Movement Speed
- Attack Damage
- Heal Amount
- Bow Ownership
- First Time Flag


### Shop System
**ShopPanel.cs** manages:
- Item display and purchasing
- Gold validation
- Stat upgrades
- UI animations (fade in/out)

### Combat System
**SoldierController.cs** provides:
- Movement and jumping
- Melee and ranged attacks
- Dash mechanic with iframe
- Health management
- Spawn effects

### Enemy AI
- **Golem:** Patrol-based movement with attack detection
- **Boss:** Advanced pattern-based attacks and behavior

---

## ?? Controls Reference

### Keyboard Input
- **Movement:** WASD or Arrow Keys
- **Jump:** W / Up Arrow
- **Dash:** Spacebar
- **Attack:** Left Click
- **Bow:** Right Click
- **Shop:** E
- **Pause:** ESC
- **Menu Navigation:** Up/Down Arrow or W/S
- **Select:** Enter

---

## ?? Contributing

### Branching Strategy
- `main` - Stable release branch
- `dev/complete` - Development branch
- Feature branches: `feature/feature-name`

### Code Standards
- Follow existing code style and naming conventions
- Comment code in English and Vietnamese
- Keep methods focused and single-responsibility
- Test changes in the Unity Editor before committing

### Commit Message Format

[Type] Brief description

Detailed explanation (optional)

Type: feat, fix, refactor, docs, style, test, chore


---

## ?? License

This project is part of an educational assignment. Please check the LICENSE file for specific usage rights.

---

## ?? Support

For issues, questions, or suggestions:
1. Check existing GitHub Issues
2. Create a new Issue with detailed description
3. Include reproduction steps if reporting a bug

---

## ?? Credits

- **Development:** Game programming team
- **Assets:** Kenney Asset Pack, Unity Standard Assets
- **Framework:** Unity Engine, Input System

---

**Happy Gaming! ??**


This revised README.md maintains the original structure while integrating the new content seamlessly, ensuring clarity and coherence throughout the document.
