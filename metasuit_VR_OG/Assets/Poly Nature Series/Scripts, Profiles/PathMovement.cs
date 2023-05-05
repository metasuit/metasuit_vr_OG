using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



namespace AquariusMax.PolyNature {
public class PathMovement : MonoBehaviour {

		public WaypointPath pathToFollow;
        public InputActionProperty showButton;
		public Transform armPosition;
        public int currentWayPointID = 0;
		public float moveSpeed;
		public float reach = 1.0f;
		public float ArmReach = 10.0f;
		public float rotationSpeed = 0.5f;
		public string pathName;
	
        Vector3 lastPosition;
		bool Flying = true;

        private Animator anim;

        // Use this for initialization
        void Start ()
	{
            //pathToFollow = GameObject.Find (pathName).GetComponent<WaypointPath> ();
            //lastPosition = transform.position;
            anim = gameObject.GetComponentInChildren<Animator>();

        }

		// Update is called once per frame
		void Update()
		{

			if (showButton.action.WasPressedThisFrame())
			{
				if (Flying)
				{
					lastPosition = pathToFollow.pathPoints[currentWayPointID].position;
					Debug.Log(pathToFollow.pathPoints[currentWayPointID].position);
					pathToFollow.pathPoints[currentWayPointID].position = armPosition.position;
					Flying = false;
				}
				else
				{
					pathToFollow.pathPoints[currentWayPointID].position = lastPosition;
					Flying = true;
					
				}

			}



			float distance = Vector3.Distance(pathToFollow.pathPoints[currentWayPointID].position, transform.position);
			transform.position = Vector3.MoveTowards(transform.position, pathToFollow.pathPoints[currentWayPointID].position, Time.deltaTime * moveSpeed);

			var rotation = Quaternion.LookRotation(pathToFollow.pathPoints[currentWayPointID].position - transform.position);
			transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);

			if (!Flying)
			{
          
                //handle landing
                pathToFollow.pathPoints[currentWayPointID].position = armPosition.position;
                if (distance <= ArmReach)
                {
                    anim.SetInteger("AnimationPar", 2);
					//anim.SetInteger("AnimationPar", 3);
                }


            }
			else
			{
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