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

The player completes the level when:
- The countdown timer has started and is still running.
- The player has located and collected the required key item.
- The final exit door has been unlocked using the collected key.
- The player opens the exit door before the timer reaches zero / avoid reaching to 0 health bar from enemy.


<ins>Failure Condition:</ins>

The game ends and displays a Game Over screen if:
- Player HP reaches 0 after being hit by the enemy.
- The countdown timer reaches 0 before escaping.

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
Implemented the Audio System as to enhance player immersion and provide clear feedback for interactions within the game environment. The goal was to create a flexible and reusable audio structure that allowed different objects (e.g., doors, switches, pickups) to trigger sound effects without duplicating audio sources across multiple GameObjects.

Scripts:


How it Works / Implementation Summary:

- Created a centralized SoundManager using the Singleton pattern to manage all audio playback.
- Removed individual AudioSource components from interactive objects to reduce redundancy and improve performance.

Added reusable public functions for:
- PlaySFX() for one-shot sound effects
- PlayMusic() for looping background audio
- StopMusic() for victory transition

Integrated spatial audio for environmental sounds when relevant.

How to Trigger:
Players will hear audio when:

- Opening a door (Left Mouse Click when near a door)
- Activating a switch
- Picking up key objects
- Completing the level (Victory music plays automatically)
- Level Fail (Lose music plays automatically
- Player movment and receiving damagers
- Enemy detects player (Aleart warning sfx)

**2. Enemy AI (SHREK)**

Description:
Implemented the Enemy AI that introduces challenge and tension by having an enemy react to the player's presence. The enemy will detect, chase, and attack the player that has a higher chance of the player to not complete the game level. It will return to its starting point depending on player actions. This system encourages the player to stay alert and adds risk when navigating the level.

Scripts:

How to Trigger in Game:
The enemy AI activates when:

- The player enters the enemy detection zone → enemy begins chasing
- The player leaves the zone → enemy returns to original position

**3. Basic UI**

Description:
The Basic UI system provides all player-facing menus required for gameplay flow, including the Main Menu, Pause Menu, HUD, and Game Over/Victory screens. The UI allows the player to start the game, pause during gameplay, view progress, and return to the main menu after finishing or failing the level.

Scripts:


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
