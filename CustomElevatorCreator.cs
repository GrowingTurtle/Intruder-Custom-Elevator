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
    public int startingLevel = 0;
    public Vector2 ButtonSpaces;
    public Transform[] ElevatorPositions;
    [Header("Extra Info")]
    public List<AnimationClip> elevatorAnimations = new List<AnimationClip>();
    public List<Transform> ButtonCallingFloors = new List<Transform>();
    public List<Transform> ButtonFloors = new List<Transform>();

    [Header("References")]
    public GameObject buttonObject;
    public GameObject callingButton;
    public GameObject buttonBlocker;
    public GameObject callButtonBlocker;
    public Transform thisParent;
    public Transform callParent;






    public void StartMethod()
    {
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
                    


                    int duration = animationDuration * (int)Mathf.Abs(i - floorNum);


                    Activator buttonActivator = Instantiate(buttonObject, newGroup.transform).GetComponent<Activator>();
                    buttonActivator.transform.localPosition = new Vector3(0 + horizontalOffset * ButtonSpaces.x, buttonActivator.transform.localPosition.y - verticalOffset * ButtonSpaces.y, buttonActivator.transform.localPosition.z);
                    
                    buttonActivator.triggerByUse = true;
                    buttonActivator.animationNames = new string[1];
                    buttonActivator.animationNames[0] = "MoveYAnimationFrom " + floorNum + " to " + i;
                    buttonActivator.objectsToAnimate = new GameObject[1];
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
                    buttonActivator2.objectsToDisable[0] = buttonBlocker;
                    buttonActivator2.objectsToDisable[1] = callButtonBlocker;
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
                    int lastDigit = int.Parse(button.animationNames[0][button.animationNames[0].Length - 1].ToString());
                    button.objectsToEnable[2] = ButtonFloors[lastDigit].gameObject;
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
                    int duration = animationDuration * (int)Mathf.Abs(i - k);

                    Activator newButton = Instantiate(callingButton, newGroup.transform).GetComponent<Activator>();
                    newButton.objectsToAnimate = new GameObject[1];
                    newButton.objectsToAnimate[0] = this.gameObject;
                    newButton.animationNames = new string[1];
                    newButton.animationNames[0] = "MoveYAnimationFrom " + i + " to " + k;
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
                    string substring = button.animationNames[0].Substring(19, 1);
                    string finalSubstring = button.animationNames[0][button.animationNames[0].Length - 1].ToString();                   
                    foreach(Transform buttonParents in ButtonCallingFloors)
                    {
                        foreach (Transform g in buttonParents)
                        {
                            string substring2 = g.GetComponent<Activator>().animationNames[0].Substring(19, 1);      
                            if (substring2 == finalSubstring)
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
                button.objectsToEnable[t] = ButtonFloors[(int.Parse(button.animationNames[0][button.animationNames[0].Length - 1].ToString()))].gameObject;
                for(int floorButtonNum = 0; floorButtonNum < ButtonFloors.Count; floorButtonNum++)
                {
                    if(floorButtonNum != (int.Parse(button.animationNames[0][button.animationNames[0].Length - 1].ToString())))
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
                    string lastDigit = button.animationNames[0][button.animationNames[0].Length - 1].ToString();
                    for(int floorNum = 0; floorNum < ButtonCallingFloors.Count; floorNum++)
                    {
                       
                          for (int k = 0; k < ButtonCallingFloors[floorNum].childCount; k++)
                          {
                                if (floorNum != int.Parse(lastDigit))
                                {
                                    //Code runs for every single button inside of call button parents                         
                                    if (ButtonCallingFloors[floorNum].GetChild(k).GetComponent<Activator>().animationNames[0].Substring(19, 1) == lastDigit)
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
                                    if(ButtonCallingFloors[floorNum].GetChild(k).GetComponent<Activator>().animationNames[0].Substring(19, 1) != lastDigit)
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

        //SetsStartingLevel
        for (int floorNums = 0; floorNums < ButtonCallingFloors.Count; floorNums++)
        {

            foreach (Transform child in ButtonCallingFloors[floorNums])
            {
                if (floorNums != startingLevel)
                {
                    Debug.Log("Floornum != startinglevel");

                    string substring = child.GetComponent<Activator>().animationNames[0].Substring(19, 1);
                    if (int.Parse(substring) != startingLevel)
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

    }
    public void Reset()
    {
        elevatorAnimations = new List<AnimationClip>();
        ButtonFloors = new List<Transform>();
        ButtonCallingFloors = new List<Transform>();
        for(int i = thisParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(thisParent.transform.GetChild(i).gameObject);           
        }
        for (int i = callParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(callParent.transform.GetChild(i).gameObject);

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

        Keyframe[] keysY = new Keyframe[2];
        keysY[0] = new Keyframe(0f, startPos.y);
        keysY[1] = new Keyframe(d, endPos.y);

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