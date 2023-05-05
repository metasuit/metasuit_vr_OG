using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.InputSystem;



namespace AquariusMax.PolyNature
{
    public class PathMovementBirds : MonoBehaviour
    {

        public WaypointPath pathToFollow;
       // public InputActionProperty showButton;
        public Transform armPosition;
        public int currentWayPointID = 0;
        public float moveSpeed = 5f;
        public float reach = 0.25f;
        public float ArmReach = 10.0f;
        public float rotationSpeed = 1.5f;
        public string pathName;
        public float clippingThresholdAngle = 3f;

        private Vector3 lastPosition;
        public bool Flying = true;
        private float distance;
        private Animator anim;

        // Use this for initialization
        void Start()
        {
            //pathToFollow = GameObject.Find (pathName).GetComponent<WaypointPath> ();
            //lastPosition = transform.position;
            anim = gameObject.GetComponentInChildren<Animator>();
            float distance = Vector3.Distance(pathToFollow.pathPoints[currentWayPointID].position, transform.position);
        }

        // Update is called once per frame
        void Update()
        {
            /*
            if (showButton.action.WasPressedThisFrame())
            {
                if (Flying)
                {
                    //saves position of node and sets position of node to position of arm
                    lastPosition = pathToFollow.pathPoints[currentWayPointID].position;
                    pathToFollow.pathPoints[currentWayPointID].position = armPosition.position;
                    Flying = false;
                }
                else
                {
                   
                    //set back the node to its original position and starts take off
                    pathToFollow.pathPoints[currentWayPointID].position = lastPosition;
                    Flying = true;
                    anim.CrossFadeInFixedTime("Tucan_take_off", 0.3f);
                    //anim.CrossFadeInFixedTime("Run", 0.1f);
                    anim.SetInteger("AnimationPar", 5);


                }

            }

*/
            
           
          
            // code for bird when stationary
            if (!Flying)
            {
                
                //handle landing

                // set position of node to arm
                pathToFollow.pathPoints[currentWayPointID].position = armPosition.position;
                distance = Vector3.Distance(pathToFollow.pathPoints[currentWayPointID].position, transform.position);
                var rotation = Quaternion.LookRotation(pathToFollow.pathPoints[currentWayPointID].position - transform.position);


                // Start idle animation if within reach
                if (distance <= reach)
                {
                    
                    anim.SetInteger("AnimationPar", 3);
                    var armRotation = Quaternion.LookRotation(armPosition.forward);
                    var angle = Vector3.Angle(transform.forward, armPosition.forward);
                    if(angle > clippingThresholdAngle)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, armRotation, Time.deltaTime * rotationSpeed);
                    }
                    else
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, armRotation, Time.deltaTime * rotationSpeed * 25);
                    }

                    // clip position to arm
                    transform.position = armPosition.position;
                    //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
                }
                // Start landing animation if within ArmReach and take direct line
                else if (distance <= ArmReach)
                {
                    anim.SetInteger("AnimationPar", 2);
                    transform.position = Vector3.MoveTowards(transform.position, pathToFollow.pathPoints[currentWayPointID].position, Time.deltaTime * moveSpeed);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
                }
                else
                {
                    //default rotation and speed outside of armReach
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * moveSpeed);

                        // rotation if angle is small enough but still outside of reach
                        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
                }

            }
            else
            {
                distance = Vector3.Distance(pathToFollow.pathPoints[currentWayPointID].position, transform.position);
                var rotation = Quaternion.LookRotation(pathToFollow.pathPoints[currentWayPointID].position - transform.position);

                // rotation if angle is small enough but still outside of reach
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);



                if (distance <= ArmReach)
                {
                    transform.position = Vector3.MoveTowards(transform.position, pathToFollow.pathPoints[currentWayPointID].position, Time.deltaTime * moveSpeed);
                }
                else
                {
                    // position update if not within armReach
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime * moveSpeed);
                }
               
                //update IDs
                if (distance <= reach)
                {
                    currentWayPointID++;
                }

                if (currentWayPointID >= pathToFollow.pathPoints.Count)
                {
                    currentWayPointID = 0;
                }
            }

        }
    }
}