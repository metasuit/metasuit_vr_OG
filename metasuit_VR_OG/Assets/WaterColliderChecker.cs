using System.IO.Ports;
using UnityEngine;

public class WaterColliderChecker : MonoBehaviour
{
    public string portName = "COM3"; // Define the serial port to use
    public int baudRate = 9600; // Define the baud rate to use
    public int numberToSend = 80; // Define the number to send when the camera is within a collider

    private SerialPort serialPort;

    private void Start()
    {
        // Initialize the serial port
        serialPort = new SerialPort(portName, baudRate);
        serialPort.Open();
    }

    private void Update()
    {
        // Check whether the main camera is within at least one of the colliders
        if (Physics.CheckBox(transform.position, transform.localScale / 2))
        {
            // Write the defined number to the serial port
            serialPort.Write(numberToSend.ToString());
        }
    }

    private void OnDestroy()
    {
        // Close the serial port when the script is destroyed
        serialPort.Close();
    }
}