# **Unity Project – Sprint 3**

## **Overview**

This project represents **Sprint 3** of our Unity game development process.  
The game is developed using **Unity 6.0 LTS**.

At this stage, the project contains:

* Multiple scenes for the various systems that were developed  
* Basic gameplay systems:
  * Player movement (walking, sprinting, and sneaking) using `CharacterController` and the **Unity Input System**
  * Sneaking: hold **Ctrl**
  * Sprinting: hold **Shift**
* Third-person camera that follows the player and avoids obstacles
* Player attack system
* Player health, displayed with a health bar that changes color depending on remaining health
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
  * Stealing from enemies: player must be close to the enemy and press **E** to attempt stealing
* Player inventory system:
  * Items can be picked up via interaction
  * Items are added to the player’s inventory, which can be opened/closed with **I**
* Item and interaction systems are easily expandable using **ScriptableObjects** and **interfaces**
* **Stealing system**:
  * Player can steal from an enemy by getting close and pressing **E**
  * Player sneak/sprint affect detection (sneak: hold **Ctrl**, sprint: hold **Shift**)
* **Simple dialog system**:
  * Dialog can be advanced by pressing **Space**
* **Juice additions**, including:
  * Outlines for interactables
  * Tween-based UI/element animations
  * Player model added
  * Player animations added and hooked up
* A scene called **`JuiceScene`** where all current mechanics can be tested

### **Lockpick Feature (Separate Branch)**

A lockpicking prototype exists on the **`Lockpick`** branch.  
It can be tested in **`SampleScene`** on that branch.

The lockpick system currently includes:

* A random number of lockpick uses/attempts
* Player can move left/right to search for the correct lockpick spot
* Failure triggers a lockpick-fail animation and other feedback

---

## **How to Run**

1. Open the project in **Unity 6.0 LTS**  
2. Navigate to **`JuiceScene`** (for current sprint)  
   * To test the lockpick prototype, switch to the **`Lockpick`** branch and open **`SampleScene`**  
3. Press **Play** in the Unity Editor  
4. Set the game resolution to **1920×1080** (inventory UI does not currently scale)  
5. Click on the Game view to lock the cursor

> No additional setup is required at this stage.

---

## **Sprint Artifacts**

A folder named **`Reports`** is included in the project.  
It contains sprint-related documentation.

---

## **Controls Summary**

* **E** — Interact / Steal (when near an enemy)  
* **I** — Open/close inventory  
* **Ctrl** — Sneak (hold)  
* **Shift** — Sprint (hold)  
* **Space** — Advance dialog

---

## **Review Feedback**

Please send the report feedback **as a message on Microsoft Teams**.
