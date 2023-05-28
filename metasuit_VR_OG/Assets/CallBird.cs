using AquariusMax.PolyNature;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;

public class CallBird : MonoBehaviour
{

    public InputActionProperty showButton;
    public Transform armPosition;
    public Transform elbowPosition;
    public WaypointPath[] wayPointpaths;
    public GameObject[] birds;
    public GameObject bird;
    public PathMovementBirds[] childScripts;
    public ChangeMaterial changeMaterialScript;
    public SerialCommunicator communicator;
    public float moveTowardsElbow = 1f;

    public Vector3 lastPosition;
    public int index_bird = 0;
    private Animator anim;
    public bool call_bird = true;
    public bool Debugging = false;

    public bool polarity = false;
    public bool isZipped = false;

    private WaypointPath pathToFollow;
    private PathMovementBirds childScript;
    private int currentWayPointID;

    // Start is called before the first frame update
    void Start()
    {
        wayPointpaths = GetComponentsInChildren<WaypointPath>();
        birds = GetComponentsInChildren<GameObject>();
        childScripts = GetComponentsInChildren<PathMovementBirds>();

    }

    // Update is called once per frame
    void Update()
    {
        pathToFollow = wayPointpaths[index_bird];
        bird = birds[index_bird];
        childScript = childScripts[index_bird];
        currentWayPointID = childScript.currentWayPointID;
        anim = bird.GetComponentInChildren<Animator>();

       
        if (showButton.action.WasPressedThisFrame())
        {
            if (call_bird)
            {
                //saves position of node and sets position of node to position of arm
                lastPosition = pathToFollow.pathPoints[currentWayPointID].position;
                pathToFollow.pathPoints[currentWayPointID].position = armPosition.position + childScript.positionOffset*childScript.moveTowardsElbow + childScript.offset;
                call_bird = false;
                childScript.Flying = false;

                changeMaterialScript.objectRenderer.enabled = !changeMaterialScript.objectRenderer.enabled;
                if (changeMaterialScript.objectRenderer.enabled)
                {
                    changeMaterialScript.objectRenderer.material = changeMaterialScript.materials[index_bird];

                }
            }
            else
            {
                BirdFlyOff();
            }

        }

    }
  
    public void BirdFlyOff()
    {
        childScript.audioSourceTakeOff.Play();
        //set back the node to its original position and starts take off
        pathToFollow.pathPoints[currentWayPointID].position = lastPosition;
        call_bird = true;
        childScript.Flying = true;
        childScript.AngleClipped = false;
        childScript.withinReach = false;

        // ANIMATION
        childScript.reachedAnim = false;
        anim.CrossFadeInFixedTime("take_off", 0.3f);
        //anim.CrossFadeInFixedTime("Run", 0.1f);
        anim.SetInteger("AnimationPar", 5);
        // ANIMATION


        // ACTUATION
        if (communicator.activateZipping && isZipped)
        {
            int polarityValue = polarity ? 1 : 0;
            polarityValue += 254;
            Debug.Log(polarityValue);

            bool success = false;
            
            for(int i = 0; i < 5; i++)
            {
                while (!success)
                {
                    try
                    {

                        communicator.SendDutyCycle(polarityValue); //Set to LowLow
                        success = true;
                    }
                    catch (IOException ex)
                    {
                        // Handle the IOException here
                        Debug.LogError("IOException caught: " + ex.Message);

                    }
                }
                success = false;
            }

            for (int i = 0; i < 10; i++)
            {
                while (!success)
                {
                    try
                    {

                        communicator.SendDutyCycle(0); //Set to LowLow
                        success = true;
                    }
                    catch (IOException ex)
                    {
                        // Handle the IOException here
                        Debug.LogError("IOException caught: " + ex.Message);

                    }
                }
                success = false;
            }
            polarity = !polarity;
            isZipped = false;


        }
        // ACTUACTION

        changeMaterialScript.objectRenderer.enabled = !changeMaterialScript.objectRenderer.enabled;
        if (changeMaterialScript.objectRenderer.enabled)
        {
            changeMaterialScript.objectRenderer.material = changeMaterialScript.materials[index_bird];

        }
        //Debugging fix one bird -> comment index++
        if (!Debugging)
        {
            index_bird++;
        }


        if (index_bird == birds.Length)
        {
            index_bird = 0;
        }
       

    }
}
