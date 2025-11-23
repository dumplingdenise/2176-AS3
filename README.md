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
- E - Interactions with Objects (Picking up, Pushing Objects)
- Space Bar - Jump
- Left Mouse Click - Interaction with Open/Close doors
- Right Mouse Click - Cycle through 3 different camera angles (1st person, fixed angle, 3rd person)
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

----------
<ins>Tasks Attempted:</ins>

**Denise Teo:**
  
**Player Movement and 3D Camera**

Description:

How to Trigger:

Scripts:
PlayerMovement.cs
CameraControl.cs
CameraPivot.cs
FixedCameraZone.cs
LookInteraction.cs
LightInteraction.cs
LightGameTimer.cs


**Edris Foo:**

1. Swingable Door Interaction

Description:
Implemented a functional door that opens and closes when interacted with. The door uses a hinge-style rotation so that it behaves like a real swing door instead of sliding or teleporting. Players must be within range and use mouse input to activate the door.

Scripts:
DoorInteraction.cs

How to Trigger:
- Stand near the door.
- Press Left Mouse Click to swing the door open or closed.

2. Task Board Interaction

Description:
Created an interactive task board that allows the player to read the level objectives. When the player approaches and activates the board, the task UI appears and explains what they must complete to escape the room. This helps guide the player at the beginning of the level.

Scripts:
BoardInteraction.cs

How to Trigger:

- Look at the task board within interaction range.
- Press E to display the task UI.
- Press Esc / Close UI button to exit.

3. Locked Door With Key Unlock

Description:
Implemented a locked exit door that cannot be opened until the player has collected the required key. Before the key is obtained, interacting with the door displays a “Door is Locked” message. After the key is collected, the door can be opened normally, allowing the player to finish the level.

Scripts:
LockedDoorInteraction.cs
KeyPickUp.cs
EndTrigger.cs
VictoryUIManager.cs

How To Trigger:

- Attempt to open the locked door → UI displays “Locked” message.
- Find and pick up the key using 'E'.
- Return to the door and use Left Mouse Click to open it.

4. Countdown Timer System

Description:
Implemented a real-time countdown that gives the player a limited duration to complete the level. The timer is always visible in the HUD. If the timer reaches zero before the exit door is opened, the player automatically loses and the Game Over screen is displayed.

Scripts:
LightGameTimer.cs

How to Trigger:
- Timer begins automatically when the level starts
- Continues ticking until:
1. Player opens the exit door (victory), or
2. Timer reaches zero and triggers Game Over


**Joann Bisseling:**
1. 

**Teo Shumin:**
1.
