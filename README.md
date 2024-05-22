# Elevator System Setup Guide

## Components and Settings

### Button Object
- **Prefab**: The `Button` is a prefab with pre-configured Activator settings.
- **Calling Button**: Similar to `Button` but with different settings.

### Elevator Setup
- **Starting Level**: The initial floor where the elevator begins.
- **Elevator Positions**: Defines the `y-axis` positions the elevator will travel to.
- **H Space & V Space**: Determines the horizontal and vertical spacing for button instantiation, arranging them in a grid pattern.

### Button Blockers
- **Button Blocker & Call Button Blocker**: Objects in the scene that get enabled/disabled when the elevator starts moving, preventing button interaction.

### Automatic Configuration
- **Button Floors**, **Button Calling Floors**, and **Elevator Animations**: These fields are automatically filled when you click "Create Animation."

### Parent Objects
- **"This Parent"**: The parent object for all interior elevator buttons.
- **"Call Parent"**: The parent object for all elevator call buttons.

### Animation Duration
- **Duration Calculation**: The time taken between floors. For instance:
  - If set to 1 and moving from floor 1 to floor 5, it takes 5 seconds.
  - If set to 2 and moving from floor 3 to floor 0, it takes 6 seconds (3 x 2).

### Reset Button
- **Temporary Function**: Clears all buttons under "This Parent" and resets the lists for Button Floors, Button Calling Floors, and Elevator Animations.

## Important Notes
- **Local Space**: Ensure elevator positions and the elevator object share the same parent for accurate local space positioning.    
![Elevator Setup](https://i.imgur.com/pvZYr3F.png)

- **Elevator Buttons Parent**: Place the elevator buttons' parent inside the elevator object so they move together.    
![Button Setup](https://i.imgur.com/WpEZFN8.png)

## Animation Folder
- **Directory**: Create a folder named `ElevatorAnimation` in the `Assets` directory for animation files. This can be changed in the editor script if needed.

## Manual Setup Without Prefabs

1. **Create Button Object**: 
   - Create a 3D cube named `Button`.
   - Attach an `Activator` script.
   - Enable `On Use` and set `After Time` to -1.     
   ![Activator Setup](https://i.imgur.com/HEhsMUP.png)

2. **Add Second Activator**:
   - Attach another `Activator` script.
   - Set `After Time` to -1 and `On Use` to false.

3. **Activate Events**:
   - In the `Activate Events` section of the first activator, attach the same object and call `Activator(1).Activate`.      
   ![Activate Event](https://i.imgur.com/G8mmZeC.png)

4. **Add Text Mesh**: Attach a text mesh as a child of the button object.

5. **Duplicate for Calling Button**:
   - Duplicate the `Button` object.
   - Rename it to `CallingButton` and adjust as needed.

6. **Assign to Elevator Script**: Assign both buttons to the respective slots in the Elevator Script.
