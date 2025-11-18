# **Unity Project – Sprint 2**

## **Overview**

This project represents **Sprint 2** of our Unity game development process.
The game is developed using **Unity 6.0 LTS**.

At this stage, the project contains:

* Multiple scenes for the various systems that were developed
* Basic gameplay systems:

  * Player movement (walking, sprinting, and sneaking) using `CharacterController` and the **Unity Input System**
* Third-person camera that follows the player and avoids obstacles

  * Player attack system
* Player health, displayed with a health bar that also changes color depending on remaining health
* Dummy enemy that:

  * Has health
  * Takes damage when attacked
  * Gets disabled when health reaches zero
* Real enemy with:

  * NavMesh-based pathfinding
  * State machine defining enemy behavior
  * Investigates if the player makes noise in the patrol area (when not sneaking)
  * Begins chasing the player if line of sight is gained during investigation
  * Investigates last known player location if LOS is broken
  * Attacks the player when close enough (player can dodge by moving quickly)
* Player interaction system (using **E**):

  * Includes a door and chest interactable (currently only prints messages)
* Player inventory system:

  * Player can pick up items via interaction
  * Items are added to the player’s inventory, which can be opened/closed with **I**
* Item and interaction systems are easily expandable using **ScriptableObjects** and **interfaces**
* A scene called **`Sprint2Scene`** where all current mechanics can be tested

---

## **How to Run**

1. Open the project in **Unity 6.0 LTS**
2. Navigate to **`Sprint2Scene`**
3. Press **Play** in the Unity Editor
4. Set the game resolution to **1920×1080** (inventory UI does not currently scale)
5. Click on the Game view to lock the cursor

> No additional setup is required at this stage.

---

## **Sprint Artifacts**

A folder named **`Reports`** is included in the project.
It contains sprint-related documentation.

---

## **Review Feedback**

Please send the report feedback **as a message on Microsoft Teams**.

---