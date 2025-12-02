🎮 LTG Final Project – 2D Action Game

A fast-paced 2D action game with dynamic combat, shop upgrades, enemy AI, boss fights, and persistent progression.

<p align="center"> <img src="https://img.shields.io/badge/Engine-Unity-000000?style=for-the-badge&logo=unity" /> <img src="https://img.shields.io/badge/C#-4.7.1-239120?style=for-the-badge&logo=csharp" /> <img src="https://img.shields.io/badge/Platform-PC-0066FF?style=for-the-badge" /> <img src="https://img.shields.io/badge/Status-Active-success?style=for-the-badge" /> </p>
📌 Table of Contents

Overview

Features

Game Mechanics

Installation

Project Structure

Key Systems

Controls

Contributing

License

Support

Credits

🧩 Overview

LTG Final Project is a Unity-based 2D action game where players battle enemies, collect rewards, upgrade stats, unlock weapons, and challenge bosses.

🎯 Game Highlights

Smooth combat & movement

Shop & stat upgrades

Gold & reward drops

Advanced enemy AI

Persistent save system

🔗 Repository:
👉 https://github.com/huycq2004/LTG_FinalProject

✨ Features
🗡️ Core Gameplay

⚔️ Combat: melee, dash (iframe), bow shooting

🦘 Movement: double jump, smooth horizontal movement

🎯 Ranged Combat: arrow projectiles

❤️ Health System: dynamic UI and damage handling

🏪 Game Systems

🛒 Shop System: upgrade stats & buy bow

💰 Currency System: gold tracking

🎁 Reward System: item drops

💾 PlayerPrefs Save: persistent progress

🤖 Enemy AI: Golem & Boss behavior

🖥️ UI & UX

🏠 Main Menu

⏸️ Pause Menu

💀 Game Over Screen

📊 Real-time stat display

⚙️ Game Mechanics
🎮 Player Controls
Action	Input
Move	A/D or Arrow Keys
Jump	W / Up Arrow
Double Jump	W / Up (in air)
Dash (iframe)	Space
Melee Attack	Left Click
Bow Attack	Right Click
Shop	E
Pause	ESC
🛠️ Installation
📦 Prerequisites

Unity 2021.3 LTS or newer

.NET Framework 4.7.1

Git (optional)

📥 Clone Project
git clone https://github.com/huycq2004/LTG_FinalProject.git
cd LTG_FinalProject

▶️ Open in Unity

Open Unity Hub

Click Add Project

Select the folder

Open with Unity 2021.3+

Load the START.unity scene

Press Play

📁 Project Structure
Assets/
├── Scripts/
│   ├── Data/
│   │   └── PlayerDataManager.cs
│   ├── UI/
│   │   ├── GameManager.cs
│   │   ├── GameOverManager.cs
│   │   └── HealthBarUI.cs
│   ├── Soldier/
│   │   ├── SoldierController.cs
│   │   └── PlayerArrow.cs
│   ├── Shop/
│   │   └── ShopPanel.cs
│   ├── Reward/
│   │   └── RewardItem.cs
│   ├── Enemy/
│   │   ├── EnemyController.cs
│   │   ├── GolemController.cs
│   │   └── BossController.cs
│   └── Manager/
│       └── CurrencyManager.cs
├── Scenes/
│   ├── START.unity
│   └── MAIN SCENE.unity
├── Prefabs/
│   ├── Player
│   ├── Enemies
│   ├── Arrows
│   └── UI
└── Assets/
    ├── Sprites
    ├── Audio
    └── Fonts

🧠 Key Systems
💾 Data Persistence (PlayerPrefs)

PlayerDataManager.cs stores:

Gold

Max Health

Current Health

Movement Speed

Attack Damage

Bow Ownership

First-Time Flag

🛒 Shop System

ShopPanel.cs:

Purchase validation

Stat upgrades

UI animations

⚔️ Combat System

SoldierController.cs:

Movement, jump, dash

Attack (melee + ranged)

Damage handling

👹 Enemy AI

GolemController.cs – patrol, detect, attack

BossController.cs – advanced patterns

🎮 Controls Reference

🧭 Navigation

Move: A / D

Jump: W

Dash: Space

Attack: Left Click

Bow: Right Click

Pause: ESC

Shop: E

🤝 Contributing
🪵 Branch Structure

main — Stable version

dev/complete — Active development

feature/<name> — New feature

📌 Commit Format
[type] Short description

Optional details...


Types: feat, fix, refactor, docs, style, test, chore

📜 License

This project is part of an educational assignment.
Check the LICENSE file for details.

💬 Support

Check GitHub Issues

Create a new issue with steps & screenshots

🎨 Credits

Development: GROUP 2

Assets: Kenney, Unity Asset Store

Engine: Unity

<p align="center"> <b>Made with ❤️ using Unity</b><br><br> <img src="https://media.tenor.com/2roX3uxz_7sAAAAC/cat-computer.gif" width="200"> </p>
