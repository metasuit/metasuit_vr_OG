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
        public Transform elbowPosition;
        public Vector3 offset;
        public int currentWayPointID = 0;
        public float moveSpeed = 5f;
        public float reach = 0.25f;
        public float ArmReach = 10.0f;
        public float rotationSpeed = 1.5f;
        public string pathName;
        public float clippingThresholdAngle = 3f;
        public float speedNearPlayer = 0.8f;
        public float ArmReachAnim = 10f;
        public float landingClipReach = 0.05f;
        public float deacceleration = 0.995f;

        private Vector3 lastPosition;
        public bool Flying = true;
        public bool Clipped = false;
        public bool reached = false;
        public bool LandingClipped = false;
        private float distance;
        private Animator anim;
        private float origSpeedNearPlayer;
        public float moveTowardsElbow = 0.3f;
        public Vector3 positionOffset;



        // Use this for initialization
        void Start()
        {
            //pathToFollow = GameObject.Find (pathName).GetComponent<WaypointPath> ();
            //lastPosition = transform.position;
            anim = gameObject.GetComponentInChildren<Animator>();
            float distance = Vector3.Distance(pathToFollow.pathPoints[currentWayPointID].position, transform.position);

            origSpeedNearPlayer = speedNearPlayer;
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
                positionOffset = Vector3.Lerp(armPosition.position, elbowPosition.position, moveTowardsElbow);

                // set position of node to arm
               // positionOffset = armPosition.InverseTransformPoint(elbowPosition.position);
                Debug.Log("sdf,jkhskfh" + positionOffset);

                pathToFollow.pathPoints[currentWayPointID].position = positionOffset + offset;
                Debug.Log("ArmPosition: " + armPosition.position);
                Debug.Log("Offset: " + offset);
                Debug.Log("positionOffset" + positionOffset);
                Debug.Log("joint:" + pathToFollow.pathPoints[currentWayPointID].position);
                distance = Vector3.Distance(pathToFollow.pathPoints[currentWayPointID].position, transform.position);
                var rotation = Quaternion.LookRotation(pathToFollow.pathPoints[currentWayPointID].position - transform.position);


                // Start idle animation if within reach
                if (distance <= ArmReachAnim && !reached)
                {
                    //anim.SetInteger("AnimationPar", 2);
                    anim.CrossFadeInFixedTime("touch_down", 0.5f);
                    reached = true;
                }
                if (distance <= reach)
                {
                    
                    anim.SetInteger("AnimationPar", 3);
                    var armRotation = Quaternion.LookRotation(armPosition.forward);
                    var angle = Vector3.Angle(transform.forward, armPosition.forward);
                    if (angle > clippingThresholdAngle && Clipped == false)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, armRotation, Time.deltaTime * rotationSpeed);
                    }
                    else
                    {
                        Clipped = true;
                        transform.rotation = Quaternion.Slerp(transform.rotation, armRotation, Time.deltaTime * rotationSpeed * 25);
                    }

                    // clip position to arm
                    if(distance <= landingClipReach)
                    {
                            transform.position = positionOffset + offset;
                    }
                    else
                    {
                        transform.position = Vector3.MoveTowards(transform.position, pathToFollow.pathPoints[currentWayPointID].position, Time.deltaTime * moveSpeed * speedNearPlayer);
                    }

                    //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
                }

                // Start landing animation if within ArmReach and take direct line

                else if (distance <= ArmReach)
                {
                    
                    transform.position = Vector3.MoveTowards(transform.position, pathToFollow.pathPoints[currentWayPointID].position, Time.deltaTime * moveSpeed * speedNearPlayer);
                    if (speedNearPlayer > 0.1f) { speedNearPlayer *= deacceleration; }
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
                speedNearPlayer = origSpeedNearPlayer;
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