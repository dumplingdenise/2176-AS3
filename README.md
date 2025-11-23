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
- Timer Countdown: Ticking sound planned for when the timer reaches 10 seconds.
  

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
LookInteraction.cs, LightInteraction.cs, LightGameTimer.cs

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
BoardInteraction.cs

How to Trigger:

- Look at the task board within interaction range.
- Press E to display the task UI.
- Press Esc / Close UI button to exit.

**3. Locked Door With Key Unlock**

Description:
Implemented a locked exit door that cannot be opened until the player has collected the required key. Before the key is obtained, interacting with the door displays a “Door is Locked” message. After the key is collected, the door can be opened normally, allowing the player to finish the level.

Scripts:
LockedDoorInteraction.cs, KeyPickUp.cs, EndTrigger.cs, UIManager.cs

How To Trigger:

- Attempt to open the locked door → UI displays “Locked” message.
- Find and pick up the key using 'E'.
- Return to the door and use Left Mouse Click to open it.

**4. Countdown Timer System**

Description:
Implemented a real-time countdown that gives the player a limited duration to complete the level. The timer is always visible in the HUD. If the timer reaches zero before the exit door is opened, the player automatically loses and the Game Over screen is displayed.

Scripts:
UIManager.cs

How to Trigger:
- Timer begins automatically when the level starts
- Continues ticking until:
1. Player opens the exit door (victory), or
2. Timer reaches zero and triggers Game Over

------------------------------------------

**Joann Bisseling:**


------------------------------------------

**Teo Shumin:**


**1. Pushable Object Interaction**

Description:
Implemented a push interaction that allows the player to interact and push objects in the game. The pushable objects starts in the locked state. Player must enter the push zone, and press "E" to enable pushing. Once the player activates the interaction, the object becomes physically movable using Unity's Rigidbody system.

Scripts:
PushInteraction.cs, PushInteractionUI.cs, PlayerPush

How to Trigger:
- Walk towards object.
- Text Prompt appear when entering push area.
- Press "E" to start interacting.
- Start Pushing the object.
