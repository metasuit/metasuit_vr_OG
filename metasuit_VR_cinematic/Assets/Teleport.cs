using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.XR;

public class Teleport : MonoBehaviour
{
    public GameObject ControllerLeft;
    public GameObject ControllerRight;
    public GameObject Kyle;
    public XRNode controllerNode = XRNode.LeftHand; // The controller node to detect input from
    public KeyCode teleportButton = KeyCode.JoystickButton2; // The button to trigger teleportation
    public Transform teleportDestination; // The location to teleport to
    public Transform originDestination;
    public float teleportDelay = 1.0f;
    private float lastTeleportTime;
    bool atOrigin = true;

    void Update()
    {
        InputDevice device = InputDevices.GetDeviceAtXRNode(controllerNode);

        // Check if teleport button is pressed
        if (Input.GetKeyDown(teleportButton) || device.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryButtonValue) && primaryButtonValue)
        {
            if (Time.time - lastTeleportTime >= teleportDelay)
            {
                if (atOrigin == false)
                {
                    transform.position = teleportDestination.position;
                    ControllerLeft.SetActive(false);
                    ControllerRight.SetActive(false);
                    Kyle.SetActive(true);
                    atOrigin = true;
                }
                else
                {
                    transform.position = originDestination.position;
                    ControllerLeft.SetActive(true);
                    ControllerRight.SetActive(true);
                    Kyle.SetActive(false);
                    atOrigin = false;

                }
                lastTeleportTime = Time.time;
            }
            // Teleport to destination
            //transform.position = teleportDestination.position;
        }

    }
}