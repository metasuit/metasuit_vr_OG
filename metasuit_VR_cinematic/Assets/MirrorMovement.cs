using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class MirrorMovement : MonoBehaviour
{

    public Transform objectToFollow;
    public Transform mirror;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 objectToFollowLocal = mirror.InverseTransformPoint(objectToFollow.position);
        transform.position = mirror.TransformPoint(new Vector3(objectToFollowLocal.x, objectToFollowLocal.y, -objectToFollowLocal.z));

        Vector3 lookatmirror = mirror.TransformPoint(new Vector3(-objectToFollowLocal.x, objectToFollowLocal.y, objectToFollowLocal.z));
        transform.LookAt(lookatmirror);

    }
}
