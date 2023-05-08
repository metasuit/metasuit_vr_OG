using AquariusMax.PolyNature;
using System.Collections;
using System.Collections.Generic;
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
    public float moveTowardsElbow = 1f;

    private Vector3 lastPosition;
    public int index_bird = 0;
    private Animator anim;
    public bool call_bird = true;
    public bool Debugging = false;



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
        WaypointPath pathToFollow = wayPointpaths[index_bird];
        bird = birds[index_bird];
        PathMovementBirds childScript = childScripts[index_bird];
        int currentWayPointID = childScript.currentWayPointID;
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
            }
            else
            {

                //set back the node to its original position and starts take off
                pathToFollow.pathPoints[currentWayPointID].position = lastPosition;
                call_bird = true;
                childScript.Flying = true;
                childScript.Clipped = false;
                childScript.reached = false;
                anim.CrossFadeInFixedTime("take_off", 0.3f);
                //anim.CrossFadeInFixedTime("Run", 0.1f);
                anim.SetInteger("AnimationPar", 5);

                //Debugging fix one bird -> comment index++
                if (!Debugging)
                {
                    index_bird++;
                }
                

                if(index_bird == birds.Length)
                {
                    index_bird = 0;
                }


            }

        }

    }
}
