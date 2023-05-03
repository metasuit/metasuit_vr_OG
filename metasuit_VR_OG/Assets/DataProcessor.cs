using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataProcessor : MonoBehaviour
{
    public string fileName = @"C:\tmp\values_muxes.txt";
    public int bufferSize = 5;
    public int dataSize = 7;
    public List<List<double>> data = new List<List<double>>(); // A list of lists to hold the data

    private string currentFileContent;
    private List<double> filteredValues = new List<double>(); // A list with the 7 filtered values
    private double[] array = new double[7];


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = 1.0;
        }
        // Initialize the sensor data matrix with empty lists
        for (int i = 0; i < bufferSize; i++)
        {
            List<double> row = new List<double>();
            for (int j = 0; j < dataSize; j++)
            {
                row.Add(0.0);
            }
            data.Add(row);
        }

        // Initialize the filtered data vector
        for (int i = 0; i < dataSize; i++)
        {
            filteredValues.Add(0.0);
        }

        // For debugging print the data matrix
        for (int i = 0; i < bufferSize; i++)
        {
            // Iterate over each element in the row
            for (int j = 0; j < dataSize; j++)
            {
                // Print the element at the current row and column
                Console.Write(data[i][j] + " ");
            }
            // Start a new line after each row
            Console.WriteLine();
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

            // Read the first line of the file
            string line = sr.ReadLine();

            // Check if the line is null or empty
            if (string.IsNullOrEmpty(line))
            {
                // Handle the case where the file is empty
                // ...
            }
            else
            {
                string[] values = line.Split(',');

                for (int i = 0; i < values.Length; i++)
                {
                    array[i] = double.Parse(values[i]);
                }

                // Do something with the array of double values
                // ...
                // Do something with the array of double values
                // ...
            }
            // Split the line into an array of double values

        }
        /*
        // Open the file
        FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

        // Create a stream reader to read the file
        StreamReader reader = new StreamReader(fileStream);

        // Read the current file content
        currentFileContent = reader.ReadLine();

        // Parse the current file content into an array of values
        string[] values = currentFileContent.Split(',');
        */
        //Check if the buffer is full
        bool bufferFull = false;
        if (data.Count >= bufferSize)
        {
            bufferFull = true;
        }

        Console.WriteLine("ahdflkjahd");
        // Add the current values to the data buffer
        Transpose(ref data);
        for (int i = 0; i< dataSize; i++)
        {

            double value = array[i];
            if(bufferFull)
            {
                data[i].RemoveAt(0);
            }
            //data[i].Add(value);
            data[i].Add(value);
        }
        Transpose(ref data);

        // Apply the moving average filter to the data
        for (int i = 0; i< dataSize; i++)
        {
            filteredValues[i] = MovingAverage(data, i);
        }

        // Close the file and dispose of the reader and file stream
        //reader.Close();
        //fileStream.Dispose();

        // Restart the coroutine to continuously read and process the data
        StartCoroutine(ReadAndProcessData());
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

    public static void Transpose<T>(ref List<List<T>> matrix)
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





    public double GetFilteredValue(int index)
    {
        // Return filtered value at given index
        return array[index];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
