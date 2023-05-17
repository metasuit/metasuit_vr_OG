using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataProcessor : MonoBehaviour
{
    public string fileName = @"C:\tmp\values_muxes.txt";
    public bool applyMovingAverageToAngles = true;

    private double[] imp_meas = new double[7];
    private double[] angle_meas = new double[4];

    private int bufferSize2 = 4; // Windowsize of moving average filter for angle measurements
    private int dataSize2 = 4; // number of angle measurement channels
    private List<List<double>> mavgData2 = new List<List<double>>(); // A list of lists to hold the data
    private List<double> filteredValuesAngles = new List<double>(); // A list with the filtered angle measurements

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

        if(applyMovingAverageToAngles){
            // Initialize moving average structure for angle tracking
            for (int i = 0; i < bufferSize2; i++)
            {
                List<double> row = new List<double>();
                for (int j = 0; j < dataSize2; j++)
                {
                    row.Add(0.0);
                }
                mavgData2.Add(row); 
            }
            // Initialize list for filtered angles
            for (int i = 0; i < dataSize2; i++)
            {
                filteredValuesAngles.Add(0.0); 
            }
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

        if (applyMovingAverageToAngles)
        {
            Transpose(ref mavgData2); // Transpose empty structure
            for (int i = 0; i < dataSize2; i++)
            {

                double value = angle_meas[i]; // Get newest angle measurement for each body part

                mavgData2[i].RemoveAt(0); // Remove oldest angle measurement

                //data[i].Add(value);
                mavgData2[i].Add(value); // Append new angle measurement to buffer
            }
            Transpose(ref mavgData2);

            for (int i = 0; i < dataSize2; i++)
            {
                filteredValuesAngles[i] = MovingAverage(mavgData2, i); // Apply moving average to angle measurements
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
        if (applyMovingAverageToAngles)
        {
            return filteredValuesAngles[index];
        }
        else { 
            return angle_meas[index];
        }
    }

    private static void Transpose<T>(ref List<List<T>> matrix)
    {
        int numRows = matrix.Count;
        int numCols = matrix[0].Count;

        var transposed = new List<List<T>>();

        for (int i = 0; i < numCols; i++)
        {
            var row = new List<T>();

            for (int j = 0; j < numRows; j++)
            {
                row.Add(matrix[j][i]);
            }

            transposed.Add(row);
        }

        matrix = transposed;
    }

    private double MovingAverage(List<List<double>> data, int colIndex)
    {
        double sum = 0;
        for (int i = 0; i < data.Count; i++)
        {
            sum += data[i][colIndex];
        }
        sum /= data.Count;
        return sum;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
