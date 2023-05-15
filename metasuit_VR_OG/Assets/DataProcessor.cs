using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataProcessor : MonoBehaviour
{
    public string fileName = @"C:\tmp\values_muxes.txt";

    private double[] imp_meas = new double[7];
    private double[] angle_meas = new double[4];


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < imp_meas.Length; i++)
        {
            imp_meas[i] = 1.0;
        }
        for (int i = 0; i < imp_meas.Length; i++)
        {
            imp_meas[i] = 1.0;
        }

        // Start a coroutine to continuously read and process the data from the txt file
        StartCoroutine(ReadAndProcessData());

    }

    private IEnumerator ReadAndProcessData()
    {
        // Wait for one frame to ensure that all Start methods have been called
        yield return null;

        FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using (StreamReader sr = new StreamReader(fileStream))
        {
            string line = sr.ReadLine();
            // Check if the line is null or empty
            if (string.IsNullOrEmpty(line))
            {
                Debug.Log("No calibration data in file value_muxes");
            }
            else
            {
                string[] values = line.Split(',');

                for (int i = 0; i < values.Length; i++)
                {
                    imp_meas[i] = double.Parse(values[i]);
                }
            }
        }

        FileStream fileStream2 = new FileStream(@"C:\tmp\values_angles.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using (StreamReader sr = new StreamReader(fileStream2))
        {
            string line = sr.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                Debug.Log("No calibration data in file values_angles");
            }
            else
            {
                // Get last calibration values
                string[] values = line.Split(',');
                for (int i = 0; i < values.Length; i++)
                {
                    angle_meas[i] = double.Parse(values[i]);
                }
                

            }
        }

        // Restart the coroutine to continuously read and process the data
        StartCoroutine(ReadAndProcessData());
    }

    public double GetImpedance(int index)
    {
        // Return filtered value at given index
        return imp_meas[index];
    }
    public double GetAngle(int index)
    {
        // Return filtered value at given index
        return angle_meas[index];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
