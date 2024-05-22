using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

#if (UNITY_EDITOR)

[ExecuteInEditMode]
public class CustomElevatorCreator : MonoBehaviour
{
    [Header("Options")]
    public int animationDuration = 1;
    public int doorAnimationDuration;
    public int startingLevel = 0;
    public Vector2 ButtonSpaces;
    public Transform[] ElevatorPositions;
    [Header("Extra Info")]
    public List<AnimationClip> elevatorAnimations = new List<AnimationClip>();
    public List<Transform> ButtonCallingFloors = new List<Transform>();
    public List<Transform> ButtonFloors = new List<Transform>();
    public List<Transform> Doors = new List<Transform>(); 

    [Header("References")]
    public GameObject buttonObject;
    public GameObject callingButton;
    public GameObject buttonBlocker;
    public GameObject callButtonBlocker;
    public Transform thisParent;
    public Transform callParent;
    public Transform DoorGroupParent;
    public GameObject DoorPrefab;



    //Doors to unlock = From
    //Doors to lock = to

    public void StartMethod()
    {
        //Sets Duration For Door Animation In PrefabButtons

        Activator[] ButtonActivators = buttonObject.GetComponents<Activator>();
        ButtonActivators[2].delayTime = doorAnimationDuration;
        Activator[] CallingButtonActivators = callingButton.GetComponents<Activator>();
        CallingButtonActivators[2].delayTime = doorAnimationDuration;



        for (int i = 0; i < ElevatorPositions.Length; i++)
        {
            for (int k = 0; k < ElevatorPositions.Length; k++)
            {
                if (i != k)
                {
                    int duration = animationDuration * (int)Mathf.Abs(i - k);
                    elevatorAnimations.Add(CreateAnimation(ElevatorPositions[i].localPosition, ElevatorPositions[k].localPosition, i, k, duration));
                }

            }
        }


        //Generates Inside Buttons
        for (int floorNum = 0; floorNum < ElevatorPositions.Length; floorNum++)
        {
            GameObject newGroup = new GameObject();
            newGroup.name = "Floor" + floorNum;
            newGroup.transform.parent = thisParent.transform;
            newGroup.transform.localPosition = Vector3.zero;
            ButtonFloors.Add(newGroup.transform);
            newGroup.transform.localRotation = Quaternion.identity;
            newGroup.transform.localScale = new Vector3(0.16f, 7.96f, 0.16f);
            int horizontalOffset = 1;
            int verticalOffset = 0;
            for (int i = 0; i < ElevatorPositions.Length; i++)
            {

                if (i % 2 == 0)
                    verticalOffset++;

                if (horizontalOffset > 1)
                    horizontalOffset = 0;
                if (i != floorNum)
                {
                    


                    int duration = animationDuration * (int)Mathf.Abs(i - floorNum) + doorAnimationDuration;


                    Activator buttonActivator = Instantiate(buttonObject, newGroup.transform).GetComponent<Activator>();
                    buttonActivator.transform.localPosition = new Vector3(0 + horizontalOffset * ButtonSpaces.x, buttonActivator.transform.localPosition.y - verticalOffset * ButtonSpaces.y, buttonActivator.transform.localPosition.z);
                    
                    buttonActivator.triggerByUse = true;
                    buttonActivator.animationNames = new string[2];
                    buttonActivator.animationNames[0] = "MoveYAnimationFrom " + floorNum + " to " + i;
                    buttonActivator.GetComponent<ButtonData>().From = floorNum;
                    buttonActivator.GetComponent<ButtonData>().To = i;
                    buttonActivator.objectsToAnimate = new GameObject[2];
                    buttonActivator.objectsToAnimate[0] = this.gameObject;
                    buttonActivator.activateAfterTime = -1;
                    buttonActivator.objectsToDisable = new GameObject[1 + (int)Mathf.Pow(ElevatorPositions.Length, 2)];
                    buttonActivator.objectsToDisable[0] = ButtonFloors[floorNum].gameObject;
                    buttonActivator.objectsToEnable = new GameObject[2 + ElevatorPositions.Length];
                    buttonActivator.canRedo = true;
                    buttonActivator.redoTime = duration;
                    buttonActivator.objectsToEnable[0] = buttonBlocker;
                    buttonActivator.objectsToEnable[1] = callButtonBlocker;
                    buttonActivator.GetComponentInChildren<TextMesh>().text = i.ToString();
                    Activator[] activators = buttonActivator.gameObject.GetComponents<Activator>();

                    Activator buttonActivator2 = activators[1];
                    buttonActivator2.delayTime = duration;
                    buttonActivator2.objectsToDisable = new GameObject[2];
                   // buttonActivator2.objectsToDisable[0] = buttonBlocker;
                    //buttonActivator2.objectsToDisable[1] = callButtonBlocker;
                    buttonActivator2.activateAfterTime = -1;
                    horizontalOffset++;

                }
            }


        }

        //Sets values for inside buttons
        for (int i = 0; i < ButtonFloors.Count; i++)
        {
            foreach (Transform child in ButtonFloors[i].transform)
            {
                Activator button = child.GetComponent<Activator>();
                if (button != null)
                {
                    int to = button.GetComponent<ButtonData>().To;
                    int lastDigit = int.Parse(button.animationNames[0][button.animationNames[0].Length - 1].ToString());
                    button.objectsToEnable[2] = ButtonFloors[to].gameObject;
                    
                }
            }
        }



        //Starting Level Setup
        if(GetComponent<Activator>() == null)
        {
            gameObject.AddComponent<Activator>();
        }
        transform.position = ElevatorPositions[startingLevel].position;
        GetComponent<Activator>().activateAfterTime = 1;
        GetComponent<Activator>().objectsToDisable = new GameObject[ElevatorPositions.Length - 1 + (ElevatorPositions.Length -1) * (ElevatorPositions.Length - 1)];
        GetComponent<Activator>().objectsToEnable[0] = ButtonFloors[startingLevel].gameObject;

        int destinationIndex = 0;
        for (int i = 0; i < ElevatorPositions.Length; i++)
        {  
            if (i != startingLevel)
            {
                GetComponent<Activator>().objectsToDisable[destinationIndex] = ButtonFloors[i].gameObject;
                destinationIndex++;
            }
        }

        
        



        foreach(AnimationClip ac in elevatorAnimations)
        {
            GetComponent<Animation>().AddClip(ac, ac.name);
        }




        //Generates Call Buttons
        for(int k = 0; k < ElevatorPositions.Length; k++)
        {
            GameObject newGroup = new GameObject();
            newGroup.name = "Floor " + k + " Call Button Group";
            newGroup.transform.parent = callParent.transform;
            newGroup.transform.position = ElevatorPositions[k].position + new Vector3(2, 0.5f, 0);
            
            ButtonCallingFloors.Add(newGroup.transform);

            for (int i = 0; i < ElevatorPositions.Length; i++)
            {
                if (i != k)
                {
                    int duration = animationDuration * (int)Mathf.Abs(i - k) + doorAnimationDuration;

                    Activator newButton = Instantiate(callingButton, newGroup.transform).GetComponent<Activator>();
                    newButton.objectsToAnimate = new GameObject[2];
                    newButton.objectsToAnimate[0] = this.gameObject;
                    newButton.animationNames = new string[2];
                    newButton.animationNames[0] = "MoveYAnimationFrom " + i + " to " + k;
                    newButton.GetComponent<ButtonData>().From = i;
                    newButton.GetComponent<ButtonData>().To = k;
                    newButton.objectsToEnable = new GameObject[ElevatorPositions.Length + 1 + 2]; // the + 2 is for button blockers
                    newButton.objectsToDisable = new GameObject[ElevatorPositions.Length * (ElevatorPositions.Length - 1) + ElevatorPositions.Length];
                    newButton.GetComponentInChildren<TextMesh>().text = "Call Elevator From " + i + " to " + k;
                    Activator[] activators = newButton.gameObject.GetComponents<Activator>();

                    Activator buttonActivator2 = activators[1];
                    buttonActivator2.delayTime = duration;
                }
            }
        }

        //Sets values for call buttons
        for (int i = 0; i < ButtonCallingFloors.Count; i++)
        {
          
            //Loops through each buttons
            foreach (Transform child in ButtonCallingFloors[i].transform)
            {
                
                int t = 2;
                int k = 0;
                Activator button = child.GetComponent<Activator>();
                button.objectsToEnable[0] = buttonBlocker;
                button.objectsToEnable[1] = callButtonBlocker;
                if (button != null)
                {
                    int to = button.GetComponent<ButtonData>().To;
                    string finalSubstring = button.animationNames[0][button.animationNames[0].Length - 1].ToString();     
                    
                    foreach(Transform buttonParents in ButtonCallingFloors)
                    {
                        foreach (Transform g in buttonParents)
                        {
                            string fromSubstring = g.GetComponent<Activator>().animationNames[0].Substring(19, 1);
                            int from = g.GetComponent<ButtonData>().From;
                            
                            if (from == to) // Changed
                            {
                                button.objectsToEnable[t] = g.gameObject;
                            }
                            else
                            {
                                button.objectsToDisable[k] = g.gameObject;
                            }
                            k++;

                        }
                        t++;
                    }                     
                }

                int to2 = button.GetComponent<ButtonData>().To;


                button.objectsToEnable[t] = ButtonFloors[to2].gameObject;
                for(int floorButtonNum = 0; floorButtonNum < ButtonFloors.Count; floorButtonNum++)
                {
                    if(floorButtonNum != to2)
                    button.objectsToDisable[k] = ButtonFloors[floorButtonNum].gameObject;
                   
                    k++;
                }                          
            }
        }



        //Syncs inside buttons to call buttons
        
        for (int i = 0; i < ButtonFloors.Count; i++)
        {
            foreach (Transform child in ButtonFloors[i].transform)
            {
                //Code runs for every button in inside buttons
                Activator button = child.GetComponent<Activator>();
                int disableCounter = 2;
                int counter = 3;
                if (button != null)
                {
                   // string lastDigit = button.animationNames[0][button.animationNames[0].Length - 1].ToString();
                    int last = button.GetComponent<ButtonData>().To;
                    for(int floorNum = 0; floorNum < ButtonCallingFloors.Count; floorNum++)
                    {
                       
                          for (int k = 0; k < ButtonCallingFloors[floorNum].childCount; k++)
                          {
                                if (floorNum != last)
                                {
                                    //Code runs for every single button inside of call button parents                         
                                    if (ButtonCallingFloors[floorNum].GetChild(k).GetComponent<ButtonData>().From == last)
                                    {
                                        button.objectsToEnable[counter] = ButtonCallingFloors[floorNum].GetChild(k).gameObject;
                                        counter++;
                                    }
                                    else
                                    {
                                        button.objectsToDisable[disableCounter] = ButtonCallingFloors[floorNum].GetChild(k).gameObject;
                                    }
                                    disableCounter++;
                                }
                                else
                                {
                                    if(ButtonCallingFloors[floorNum].GetChild(k).GetComponent<ButtonData>().From != last)
                                    {
                                        button.objectsToDisable[disableCounter] = ButtonCallingFloors[floorNum].GetChild(k).gameObject;
                                        disableCounter++;
                                    }
                                }
                          }
                    }             
                }
            }
        }

        




        //Generates Doors
        
        for (int k = 0; k < ElevatorPositions.Length; k++)
        {
            GameObject newDoor = Instantiate(DoorPrefab, DoorGroupParent.transform);
            newDoor.transform.position = ElevatorPositions[k].position;
            newDoor.name = "Door Floor:" + k;
            Doors.Add(newDoor.transform);    
        }



        //Sets up door animations for every button

        //Loops through each calling buttons
        for (int i = 0; i < ButtonCallingFloors.Count; i++)
        {   
            foreach (Transform child in ButtonCallingFloors[i].transform)
            {
                Activator[] activators = child.GetComponents<Activator>();
                Activator activator = activators[0];
                int from = activator.GetComponent<ButtonData>().From;
                int to = activator.GetComponent<ButtonData>().To;
                string fromSubstring = activator.animationNames[0].Substring(19, 1);
                string toSubstring = activator.animationNames[0].Substring(activator.animationNames[0].Length - 1);
                //Sets which door to close
               
                activator.objectsToAnimate[1] = Doors[from].gameObject;
                activator.animationNames[1] = "ElevatorDoorClose";
                //Sets which door to open
                Activator delayedActivator = activators[1];
                delayedActivator.objectsToAnimate = new GameObject[1];
                delayedActivator.animationNames = new string[1];
                delayedActivator.animationNames[0] = "ElevatorDoorOpen";
                delayedActivator.objectsToAnimate[0] = Doors[to].gameObject; // changed

            }
        }

        foreach(Transform g in ButtonFloors)
        {
            foreach(Transform child in g)
            {
                Activator[] activators = child.GetComponents<Activator>();
                Activator activator = activators[0];
                int from = activator.GetComponent<ButtonData>().From;
                int to = activator.GetComponent<ButtonData>().To;
                string fromSubstring = activator.animationNames[0].Substring(19, 1);
                string toSubstring = activator.animationNames[0].Substring(activator.animationNames[0].Length - 1);
                //Sets which door to close

                activator.objectsToAnimate[1] = Doors[from].gameObject;
                activator.animationNames[1] = "ElevatorDoorClose";
                //Sets which door to open
                Activator delayedActivator = activators[1];
                delayedActivator.objectsToAnimate = new GameObject[1];
                delayedActivator.animationNames = new string[1];
                delayedActivator.animationNames[0] = "ElevatorDoorOpen";
                delayedActivator.objectsToAnimate[0] = Doors[to].gameObject;
            }
        }


        //SetsStartingLevel
        for (int floorNums = 0; floorNums < ButtonCallingFloors.Count; floorNums++)
        {
            foreach (Transform child in ButtonCallingFloors[floorNums])
            {
                if (floorNums != startingLevel)
                {
                    Debug.Log("Floornum != startinglevel");

                    string substring = child.GetComponent<Activator>().animationNames[0].Substring(19, 1);
                    int from = child.GetComponent<ButtonData>().From;
                    if (from != startingLevel)
                    {
                        Debug.Log("Added");

                        GetComponent<Activator>().objectsToDisable[destinationIndex] = child.gameObject;
                        destinationIndex++;
                    }
                }
                else
                {
                    Debug.Log("Floornum == startinglevel");
                    GetComponent<Activator>().objectsToDisable[destinationIndex] = child.gameObject;
                    destinationIndex++;
                }
            }
        }

        GetComponent<Activator>().objectsToAnimate = new GameObject[1];
        GetComponent<Activator>().animationNames = new string[1];
        GetComponent<Activator>().objectsToAnimate[0] = Doors[startingLevel].gameObject;
        GetComponent<Activator>().animationNames[0] = "ElevatorDoorOpen";


    }
    public void Reset()
    {
        elevatorAnimations = new List<AnimationClip>();
        ButtonFloors = new List<Transform>();
        ButtonCallingFloors = new List<Transform>();
        Doors = new List<Transform>();
        for(int i = thisParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(thisParent.transform.GetChild(i).gameObject);           
        }
        for (int i = callParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(callParent.transform.GetChild(i).gameObject);
        }
        for (int i = DoorGroupParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(DoorGroupParent.transform.GetChild(i).gameObject);
        }
        if (GetComponent<Animation>() != null)
        DestroyImmediate(GetComponent<Animation>());

        Animation a = gameObject.AddComponent<Animation>();
        a.playAutomatically = false;
        a.animatePhysics = true;

    }
    public AnimationClip CreateAnimation(Vector3 startPos, Vector3 endPos, int start, int end, int d)
    {
        AnimationClip clip = new AnimationClip();
        clip.name = "MoveYAnimation";

        Keyframe[] keysY = new Keyframe[3];
        keysY[0] = new Keyframe(0f, startPos.y);
        keysY[1] = new Keyframe(doorAnimationDuration, startPos.y);
        keysY[2] = new Keyframe(d + doorAnimationDuration, endPos.y);

        AnimationCurve curveY = new AnimationCurve(keysY);

        clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
        clip.legacy = true;
        AssetDatabase.CreateAsset(clip, "Assets/ElevatorAnimation/MoveYAnimationFrom " + start + " to " + end + ".anim");
        AssetDatabase.SaveAssets();

        return clip;
    }


}


[CustomEditor(typeof(CustomElevatorCreator))]
public class CustomObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CustomElevatorCreator myScript = (CustomElevatorCreator)target;

        if (GUILayout.Button("Create"))
        {
            myScript.StartMethod();
        }

        if (GUILayout.Button("Reset"))
        {
            myScript.Reset();
        }

        
    }
}
#endif