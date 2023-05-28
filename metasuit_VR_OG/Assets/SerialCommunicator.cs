using UnityEngine;
using System.IO.Ports;

public class SerialCommunicator : MonoBehaviour
{
    public string portName = "COM8"; // Define the serial port to use
    public int baudRate = 9600; // Define the baud rate to use
    public bool activateZipping = false;

    private SerialPort serialPort;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize the serial port
        serialPort = new SerialPort(portName, baudRate);

        try
        {
            serialPort.Open();
            activateZipping = true;
        }
        catch (System.Exception)
        {
            //Failed to open the serial port
            activateZipping = false;
            Debug.Log("Failed to open the serial port");
        }
    }

    public void SendDutyCycle(int Dutycycle)
    {
        serialPort.Write(Dutycycle.ToString());
        Debug.Log("Sent Dutycylce:" + Dutycycle + "to Serial");
    }

    private void OnDestroy()
    {
        // Close the serial port when the script is destroyed if it was opened
        if (serialPort != null && serialPort.IsOpen)
        {
            serialPort.Close();
        }       
    }
}
