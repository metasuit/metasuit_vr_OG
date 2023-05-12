using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using TreeEditor;
using Unity.VisualScripting;
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
        public float MinLandingSpeed = 0.5f;
        public float maxDeviationAngle = 70f;
        public CallBird callBirdScript;
        public ChangeMaterial changeMaterialScript;

        public AudioSource audioSourceLanding;
        public AudioSource audioSourceTakeOff;
        private Vector3 lastPosition;
        public bool Flying = true;
        public bool AngleClipped = false;
        public bool reachedAnim = false;
        public bool withinReach = false;
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

                pathToFollow.pathPoints[currentWayPointID].position = positionOffset + offset;
                Debug.Log("ArmPosition: " + armPosition.position);
                Debug.Log("Offset: " + offset);
                Debug.Log("positionOffset" + positionOffset);
                Debug.Log("joint:" + pathToFollow.pathPoints[currentWayPointID].position);
                distance = Vector3.Distance(pathToFollow.pathPoints[currentWayPointID].position, transform.position);
                var rotation = Quaternion.LookRotation(pathToFollow.pathPoints[currentWayPointID].position - transform.position);


                // Start idle animation if within reach
                if (distance <= ArmReachAnim && !reachedAnim)
                {
                    //anim.SetInteger("AnimationPar", 2);
                    audioSourceLanding.Play();

                    anim.CrossFadeInFixedTime("touch_down", 0.5f);
                    reachedAnim = true;
                }
                if (distance <= reach)
                {
                    withinReach = true;
                    anim.SetInteger("AnimationPar", 3);
                    Vector3 orthogonalComponent = armPosition.forward - Vector3.Project(armPosition.forward, elbowPosition.position-armPosition.position);
                    var armRotation = Quaternion.LookRotation(orthogonalComponent);

                    //Quaternion yRotation = Quaternion.FromToRotation(transform.up, Vector3.up);
                    //transform.localRotation = yRotation * transform.localRotation;
                    //transform.LookAt(transform.position + elbowPosition.forward, elbowPosition.up);
                   

                    var angle = Vector3.Angle(transform.forward, orthogonalComponent);
                    if (angle > clippingThresholdAngle && AngleClipped == false)
                    {
                        transform.rotation = Quaternion.Slerp(transform.rotation, armRotation, Time.deltaTime * rotationSpeed * 2);
                    }
                    else
                    {
                        AngleClipped = true;
                        transform.rotation = Quaternion.Slerp(transform.rotation, armRotation, Time.deltaTime * rotationSpeed * 25);
                    }

                    // clip position to arm
                    if(distance <= landingClipReach)
                    {
                            transform.position = positionOffset + offset;
                            float deviationAngle = Vector3.Angle(transform.up, Vector3.up);
                            
                            // fly off if angle with respect to global y axis to large
                            if (deviationAngle > maxDeviationAngle && AngleClipped)
                            {
                                callBirdScript.BirdFlyOff();
                            
                            Debug.Log("Deviation angle is too large: " + deviationAngle);
                                
                            }

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
                    // if leave reach after reached once fly off
                    if(withinReach) { callBirdScript.BirdFlyOff(); }
                    transform.position = Vector3.MoveTowards(transform.position, pathToFollow.pathPoints[currentWayPointID].position, Time.deltaTime * moveSpeed * speedNearPlayer);
                    if (speedNearPlayer > MinLandingSpeed) { speedNearPlayer *= deacceleration; }
                    else { callBirdScript.BirdFlyOff(); }
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


                // arm reach in this case not reach to arm but to next target node
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