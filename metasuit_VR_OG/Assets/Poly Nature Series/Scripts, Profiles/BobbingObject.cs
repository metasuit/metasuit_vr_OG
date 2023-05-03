﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;



namespace AquariusMax.PolyNature{
[System.Serializable]
public class BobbingObject : MonoBehaviour 
{
	public float amplitude;          //Set in Inspector 
	public float speed;              //Set in Inspector 
	public Vector3 tempVal;
	public Vector3 tempPos;

	void Start () 
	{			
		tempVal = transform.position;
	}

	void Update ()
	{        
		tempPos = transform.position;
		tempPos.y = tempVal.y + amplitude * Mathf.Sin (speed * Time.time);
		transform.position = tempPos;
	}	
} 
}