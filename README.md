UXG 2176: Assignment 03 - 3D Game
-
**Done By: 4D Shrek**

**Team members: Denise Teo, Edris Foo, Joann Bisseling, Teo Shumin**

<ins>Overview:</ins>

This Game is a short 3D puzzle room game. The player begins in an initial space and enters the main puzzle room,
which the player must explore and solve the interactive challenges to retrieve something within the timer countdown and avoid the enemy from receiving damages. The player must be able to unlock the final door to finish the level.
The game focuses on environmental interaction, simple logic-based puzzles, and intuitive controls to deliver a compact and engaging gameplay experience.

<ins>Game Objective:</ins>
1. Player enter the main puzzle room through the first door.
2. Explore the environment and locate interactive objects.
3. Activate switches or interacting objects to progress and unlock item pickup opportunities.
4. Collect the key item required to escape.
5. Reach and unlock the final exit door before the countdown timer expires.
6. Avoid taking fatal damage from the enemy while progressing through the room.

<ins>Intended Camera Setup:</ins>

The game features multiple camera perspectives, which the player can switch between during gameplay:

1. First-Person Camera

2. Third-Person Follow Camera

3. Fixed Angle / Static Room Camera


<ins>Controls:</ins>
- WASD - Player Movement (Forward, Backward, Left, Right)
- E - Interactions with Objects (Picking up, Pushing Objects, Turn on Light)
- Space Bar - Jump
- Left Mouse Click - Interaction with Open/Close doors
- Right Mouse Click - Cycle through 3 different camera angles (1st person, fixed angle (within the Fixed Camera Zone only), 3rd person)
- Esc - Pause Menu / Close UI Menus


<ins>Completion Condition:</ins>

The player completes the level by successfully navigating the environment and completing a series of objectives before being caught. To win, the player must:
- Explore and Interact: Locate and interact with 10 different objects (such as doors, lights, and boxes) scattered throughout the level. This is the main requirement to unlock the final victory door.
- Find the Hidden Key: Discover the key that is hidden somewhere in the level.
- Unlock the Path: Use the collected key to open a specific locked door that blocks the path leading towards the final exit area.
- Escape: After completing the 10 interactions, open the now-unlocked victory door to win the game.


<ins>Failure Condition:</ins>

The game ends and displays a Game Over screen if:
- Player HP reaches 0: The player loses all their health after being attacked three times by the main enemy AI (Shrek).

<ins>Audio System Implementation / Audio & Sound Effects Lists:</ins>

- Footsteps: Multiple footstep sounds randomly selected; plays when the player moves on the ground.
- Jump: Plays when the player jumps (press Spacebar).
- Land: Plays when the player lands on the ground after a jump or fall.
- Key Pickup: Plays when the player picks up the key (press E near key).
- Door Open: Plays when a door is opened (Left Mouse Click on unlocked door).
- Door Close: Plays when a door is closed (interact to close).
- Door Locked: Plays when the player attempts to open a locked door without a key.
- Victory: Plays when the player successfully completes the level by opening the final door with the key before the timer ends.
- Game Over: Plays when the player loses (health = 0 or timer expires).
- Button Click: Plays when the player clicks any UI button.
- Light Switch On: Plays when the player activates a light switch in the environment.
- Timer Countdown: Ticking sound planned for when the timer reaches 10 seconds to get the player feel alarmed of the remaining duration.
  

<ins>Tasks Attempted:</ins>

**Denise Teo:**
  
**1. Player Movement**

Description: 
Implemented basic Player Movement to move and jump with Animation. The player can move freely in 3D space using WASD controls. Movement direction is always based on camera orientation(Example: Pressing W moves the player forward relative to where the active camera is facing). The player uses a CharacterController, preventing diagonal speed boost and ensure collision with floors and walls.

Features implemented:

- Camera-relative WASD movement
- Proper gravity, falling & grounded checks
- No diagonal speed increase
- Uses Unity’s CharacterController for collision
- Works across all camera modes (1st person, 3rd person, fixed)

How to Trigger:

- WASD to move
- Space to jump

Scripts:
PlayerMovement.cs

**2. 3D Camera**

Description:
Implemented First Person, Third Person and Fixed Angle Camera view. Players can toggle between Third-Person and First-Person outside of Fixed Angle Zone. Within the Fixed Angle Zone, players can only toggle between the Fixed-Angle cameras in it. Both FP and TP cameras include camera collision detection to prevent clipping through walls or rotating the camera outside the environment.

**First-Person Camera**
Camera sits within the player's head and rotates based on mouse movement

**Third-Person Camera**
Camera follows behind the player at a set distance.

**Fixed-Angle Camera**
A static cinematic camera is activated when the player enters specific trigger zones. When inside a fixed camera zone, the view switches automatically.

How to Trigger:

- Walking into fixed-angle triggers swaps automatically into fixed camera.
- Walking out of trigger returns to FP/TP mode.
- FP - TP switching and Fixed-Angle Camera switching based on RMB.
- Camera collision prevention is automatic during movement and rotation.

Scripts:
CameraControl.cs, CameraPivot.cs, FixedCameraZone.cs

**3. Light Interaction**

Description:
The player can interact with lanterns on the ceiling using a raycast=based interaction system. When the player looks at the light while within range, and facing the correct angle, and interaction prompt on the lantern appears. When the player presses E, the light toggles ON, and interaction is disabled. After the light timer ends, the light turns OFF and enters a cooldown period.

During cooldown:
- Interaction prompt is hidden
- Interaction is blocked
- UI displays cooldown countdown
- After cooldown finishes, prompt reappears and player can toggle the light again.

How to Trigger:

- Look at the light while within interaction range & in the right facing angle (facing away from the lantern will not trigger)
- Press E: Lights turn on
- When the timer ends, a Cooldown starts automatically
- Cooldown ends: Interaction is re-enabled

Scripts:
LookInteraction.cs, LightInteraction.cs, UIManager.cs

------------------------------------------

**Edris Foo:**

**1. Swingable Door Interaction**

Description:
Implemented a functional door that opens and closes when interacted with. The door uses a hinge-style rotation so that it behaves like a real swing door instead of sliding or teleporting. Players must be within range and use mouse input to activate the door.

Scripts:
DoorInteraction.cs

How to Trigger:
- Stand near the door.
- Press Left Mouse Click to swing the door open or closed.

**2. Task Board Interaction**

Description:
Created an interactive task board that allows the player to read the level objectives. When the player approaches and activates the board, the task UI appears and explains what they must complete to escape the room. This helps guide the player at the beginning of the level.

Scripts:
BoardInteraction.cs, UIManager.cs

How to Trigger:

- Look at the task board within interaction range.
- Press 'E' to display the task UI panel.
- Press 'Esc' to close / exit the task board UI panel.

**3. Locked Door With Key Unlock**

Description:
Implemented a locked exit door that cannot be opened until the player has collected the required key. Before the key is obtained, interacting with the door displays a “Door is Locked” message. After the key is collected, the door can be opened normally, allowing the player to finish the level.

Scripts:
LockedDoorInteraction.cs, KeyPickUp.cs, EndTrigger.cs, UIManager.cs

How To Trigger:

- Attempt to open the locked door → UI displays “Locked” message.
- Find and pick up the key using 'E'.
- Return to the door and use Left Mouse Click to open it.

**4. Light Cooldown System**

Description:
Implemented a light-based cooldown mechanic that limits how long the player can keep a light source activated. When the player switches the light on, a cooldown timer appears in the HUD and begins decreasing in real time. Once the timer reaches zero, the light automatically switches off and the player must wait for the cooldown to replenish before turning it back on again. This adds tension and encourages strategic use of lighting during exploration.

Scripts:
UIManager.cs, LightInteraction.cs

How to Trigger:
- Cooldown begins when the player turns the light on
- Cooldown bar decreases while the light is active
- Light automatically turns off when cooldown reaches zero
- Cooldown refills only when the light is off

------------------------------------------

**Joann Bisseling:**

**1. Audio System**

Description:
Implemented a centralized Audio System to enhance player immersion and provide clear feedback for all interactions within the game. The system is built on a Singleton pattern, ensuring that all sounds are managed from a single, persistent source. This design is efficient, scalable, and keeps the game hierarchy clean by removing the need for individual AudioSource components on every interactive object. A key feature is the dynamic footstep system, which plays sounds in response to player movement.

Scripts:
AudioManager.cs

How it Works / Implementation Summary:
- Centralized Singleton Manager: Uses a static instance to allow any script to easily play sounds. The manager persists across scene loads using DontDestroyOnLoad.
- Automated AudioSource Setup: The manager automatically creates and configures AudioSource components for all sound effects, music, and footstep clips assigned in the Inspector.
- Dynamic Footstep System: A timer-based system, controlled by SetWalkingState(), plays footstep sounds from an array when the player is moving, creating realistic audio feedback.
- Simple Public Functions: Other scripts can easily play audio using straightforward calls like PlaySound("SoundName") and PlayMusic("MusicName").

How to Trigger:
Players will hear audio when:
- The player is moving (Footsteps will play automatically).
- Opening a door (DoorOpen sound).
- Activating a switch or completing a task.
- Picking up key objects.
- The enemy detects the player (EnemyNotice warning SFX).
- The player takes damage (Hurt sound).
- A UI button is clicked (ButtonClick sound).
- The level is completed (Victory music).
- The player fails the level (GameOver music).

**2. Enemy AI (SHREK)**

Description:
Implemented a state-driven Enemy AI that introduces a dynamic challenge for the player. The enemy uses a NavMeshAgent to intelligently navigate the environment. It can patrol a predefined route, detect and chase the player, and initiate attacks. A unique feature of this AI is its fleeing mechanic; it perceives certain activated lights as threats and will actively run away from them, creating strategic opportunities for the player to manipulate its behavior.

Scripts:
EnemyAI.cs

How it Works / Implementation Summary:
- State Machine: Switches between patrolling, chasing, and attacking states based on player proximity and line-of-sight.
- Intelligent Detection: Uses a Physics.Raycast to ensure the enemy has a clear line of sight before chasing, preventing it from seeing through walls.
- Fleeing Behavior: The AI's highest priority is to flee from activated safeZoneLights. When a light is a threat, it overrides all other behaviors to run to a safe distance, adding a strategic layer to gameplay.
- Attack Cooldown: An Invoke() timer manages the attack speed, ensuring fair and balanced combat encounters.

How to Trigger in Game:
- Chase: The player enters the enemy's sightRange with a clear line of sight.
- Attack: The player is within the enemy's attackRange.
- Patrol: The player is not in sight range. The enemy will follow its designated path.
- Flee: The player activates a LightInteraction object near the enemy, causing it to run away.

**3. UI & Game State Management**

Description:
A comprehensive UI and game management system that controls the entire game flow, from the main menu to the end-game screens. This system handles not only the display of information (HUD, menus) but also manages critical game states like pausing, game over conditions, and win conditions. It acts as the central hub that connects player actions, game progress, and user feedback.

Scripts:
UIManager.cs
GameManager.cs
PlayerHealth.cs
MainMenuManager.cs
FinalDoorInteraction.cs

How it Works / Implementation Summary:
- UIManager.cs: The central UI controller that manages all panels (HUD, Pause, Game Over), handles the game's pause state by controlling Time.timeScale and cursor visibility, and updates all HUD elements like health, progress bars, and timers.
- GameManager.cs: Tracks the win condition by counting unique tasks completed in a HashSet to prevent duplicate counting. It provides a global AreAllTasksComplete flag for other scripts to check.
- PlayerHealth.cs: Manages the player's health, provides a public TakeDamage() function for the enemy to call, and communicates with the UIManager to update the health display and show the game over screen.
- MainMenuManager.cs & FinalDoorInteraction.cs: These scripts handle the start and end of the game flow. The main menu uses coroutines for smooth scene loading, while the final door checks the GameManager's win condition to trigger the victory sequence.

------------------------------------------

**Teo Shumin:**


**1. Pushable Object Interaction**

Description:
Implemented a push interaction that allows the player to interact and push objects in the game. The pushable objects starts in the locked state. Player must enter the push zone, and press "E" to enable pushing. Once the player activates the interaction, the object becomes physically movable using Unity's Rigidbody system. When the player leaves the push area, the object automatically change it state to not pushable.

Scripts:
PushInteraction.cs, PushInteractionUI.cs, PlayerPush

How to Trigger:
- Walk towards object.
- Text Prompt appear when entering push area.
- Press "E" to start interacting.
- Start Pushing the object.
- Stops pushing and exit the push zone.
- Objects goes back to idle state
