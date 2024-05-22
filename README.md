#TIPS      
-Button Object is a prefab that gets instanciated. It already has some Activitator settings changed.   
-Calling Button is the same thing but with different settings.   
-Starting Level is the place the elevator begins at  
-Elevator Positions is the places on the *y axis* that the elevator will travel to.   
-H Space and V Space are for the elevator button instanciation. They get spread out in a grid like pattern and the h and v space changes how big the gaps are.  
-Button blocker and Call button blocker are the objects in the scene that are disabled/enabled when the elevator starts to move. It covers the buttons so that people cant touch them while its moving.  
-Button Floors, Button Calling Floors, and Elevator Animations are automatically filled out when you click "create animation"  
-"this parent" is the parent object for all of the interior buttons  
-"Call Parent" is the parent for the elevator calling buttons for the elevators.   
-Animation duration is how long it takes between each floor. If its 1 for example, and you go from floor 1 to floor 5, it would take 5 seconds. If its 2, and you go from 3 to 0, it would take 6 seconds. (3 x 2)  
-Reset button is temp, it currently wipes everything under "this parent" and wipes the Button Floors, Button Calling Floors and Elevator Animations lists.  


NOTE:Keep the elevator positions and the elevator object in the same place (Dont keep them in seperate parents or childs) because they use local space.  
![alt text](https://i.imgur.com/pvZYr3F.png)

