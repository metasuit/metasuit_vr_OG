﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;



namespace AquariusMax.PolyNature
{
    public class PathMovement : MonoBehaviour
    {

        public WaypointPath pathToFollow;
        public int currentWayPointID = 0;
        public float moveSpeed;
        public float reach = 1.0f;
        public float rotationSpeed = 0.5f;
        public string pathName;


        // Use this for initialization
        void Start()
        {
            //pathToFollow = GameObject.Find (pathName).GetComponent<WaypointPath> ();
            //lastPosition = transform.position;

        }

        // Update is called once per frame
        void Update()
        {


            float distance = Vector3.Distance(pathToFollow.pathPoints[currentWayPointID].position, transform.position);
            transform.position = Vector3.MoveTowards(transform.position, pathToFollow.pathPoints[currentWayPointID].position, Time.deltaTime * moveSpeed);

            
            if (pathToFollow.pathPoints[currentWayPointID].position - transform.position != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(pathToFollow.pathPoints[currentWayPointID].position - transform.position);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
            }

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