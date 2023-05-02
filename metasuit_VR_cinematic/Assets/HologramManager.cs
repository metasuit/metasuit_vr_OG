using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.InputSystem;

public class HologramManager : MonoBehaviour
{
    public Transform head;
    public float spawnDistance = 2;
    public GameObject hologram;
    //public GameObject hologram_mesh;
    public InputActionProperty showButton;
    public Vector3 hologramOffset = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (showButton.action.WasPressedThisFrame())
        {
            hologram.SetActive(!hologram.activeSelf);
            hologram.transform.position = head.position + new Vector3(head.forward.x, 0, head.forward.z).normalized * spawnDistance;
            hologram.transform.LookAt(new Vector3(head.position.x, hologram.transform.position.y, head.position.z));
            hologram.transform.forward *= -1;
            hologram.transform.position = hologram.transform.position + hologramOffset;
        }

        hologram.transform.LookAt(new Vector3(head.position.x, hologram.transform.position.y, head.position.z));
        hologram.transform.forward *= -1;
    }
}
