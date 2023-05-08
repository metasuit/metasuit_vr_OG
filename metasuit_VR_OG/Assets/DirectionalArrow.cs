using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DirectionalArrow : MonoBehaviour
{
    public GameObject arrow;
    public CallBird callBirdScript;
  
    // Update is called once per frame
    void Update()
    {
        Transform target = callBirdScript.bird.transform;
        transform.LookAt(target);
    }
}
