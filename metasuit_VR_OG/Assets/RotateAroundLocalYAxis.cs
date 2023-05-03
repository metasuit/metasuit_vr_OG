using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using Button = UnityEngine.UI.Button;

public class RotateAroundLocalYAxis : MonoBehaviour
{
    string measuredValue;
    bool calibrated = false;
    float calibrationAngle1;
    float calibrationAngle2;
    float calibrationVoltage1;
    float calibrationVoltage2;
    float voltageOffsetEstim = 1.7f;
    float voltagetoDegEstim = 300f;
    bool firstCalibrationDone = false;
    bool secondCalibrationDone = false;
    bool startOfProgram = true;
    bool measureWithOldCalibrationValues = false;

    public int RotationDirectionX = 0;
    public int RotationDirectionY = 1;
    public int RotationDirectionZ = 0;
    public int AngleOffset = 0;
    public int numberOfPeakValuesForCalibration = 10;
    public bool selfsensingTesting;
    public float calibrationTime = 2;
    public float calibrationTimeMove = 4;
    public int numberOfHasels;
    public int firstHaselIndex;
    public Button calibrateButton1;
    public Button calibrateButton2;
    public Button calibrateButton3;
    public Slider calibrationSlider1;
    public Slider calibrationSlider2;
    public Button startCalibrationButton;
    public Button startApplicationButton;
    public DataProcessor dataProcessor;
    public Toggle bodyPartToggle;

    private void Start()
    {
        // Read from File
        FileStream fileStream = new FileStream($@"C:\tmp\values_calibration_{firstHaselIndex}.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using (StreamReader sr = new StreamReader(fileStream))
        {
            string line = sr.ReadLine();
            if (string.IsNullOrEmpty(line))
            {
                Debug.Log("No calibration data in file");

            }
            else
            {
                string[] values = line.Split(',');
                calibrationVoltage1 = float.Parse(values[0]);
                calibrationVoltage2 = float.Parse(values[1]);
                calibrationAngle1 = float.Parse(values[2]);
                calibrationAngle2 = float.Parse(values[3]); ;
            }
        }

        calibrateButton1.onClick.AddListener(ButtonClick1);
        calibrateButton2.onClick.AddListener(ButtonClick2);
        calibrateButton3.onClick.AddListener(ButtonClick3);
        startCalibrationButton.onClick.AddListener(StartCalibration);
        startApplicationButton.onClick.AddListener(EndCalibration);   
    }
    void StartCalibration()
    {
        calibrated = false;
        firstCalibrationDone = false;
        secondCalibrationDone = false;
        startOfProgram = false;
        measureWithOldCalibrationValues = false;
    }
    void EndCalibration()
    {
        if (firstCalibrationDone && secondCalibrationDone)
        {
            calibrated = true;
            
        }
        else if (bodyPartToggle.isOn && startOfProgram == true)
        {
            measureWithOldCalibrationValues = true;
            StartCoroutine(ChangeApplicationButtonColor());
        }
        else Debug.Log("Calibration did not work");

    }
    void ButtonClick1()
    {
        StartCoroutine(CalibratePos1());
    }
    void ButtonClick2()
    {
        StartCoroutine(CalibratePos2());
    }
    void ButtonClick3()
    {
        StartCoroutine(CalibrateDynamic());
    }

    IEnumerator CalibrateDynamic()
    {
        startOfProgram = false;
        measureWithOldCalibrationValues = false;
        if (calibrated == false && bodyPartToggle.isOn)
        {

            calibrationAngle1 = calibrationSlider1.value;
            calibrationAngle2 = calibrationSlider2.value;
            float endTime = Time.time + calibrationTimeMove;
            List<double>[] calibrationValuesLists = new List<double>[numberOfHasels]; // array of seven lists
            float[] calibrationVoltagesPos1 = new float[numberOfHasels]; // array of 3 calibration voltages
            float[] calibrationVoltagesPos2 = new float[numberOfHasels]; // array of seven calibration voltages
            for (int i = 0; i < numberOfHasels; i++)
            {
                calibrationValuesLists[i] = new List<double>(); // initialize each list
            }

            while (Time.time < endTime)
            {
                // Debug.Log("Running concurrent task...");

                // Get filtered values                
                for (int i = 0; i < numberOfHasels; i++)
                {
                    double filteredValue = dataProcessor.GetFilteredValue(i + firstHaselIndex - 1);
                    calibrationValuesLists[i].Add(filteredValue); // add the value to the corresponding list
                }


                yield return null;
            }


            for (int i = 0; i < numberOfHasels; i++)
            {
                calibrationValuesLists[i].Sort();

                // get the 10 smallest values
                List<double> smallestValues = calibrationValuesLists[i].Take(numberOfPeakValuesForCalibration).ToList();

                // reverse the sorted list
                calibrationValuesLists[i].Reverse();

                // get the 10 largest values
                List<double> largestValues = calibrationValuesLists[i].Take(numberOfPeakValuesForCalibration).ToList();

                calibrationVoltagesPos1[i] = Convert.ToSingle(largestValues.Average());
                calibrationVoltagesPos2[i] = Convert.ToSingle(smallestValues.Average());
                //Debug.Log("Calibration voltage " + (i + 1) + ": " + calibrationVoltagesPos1[i]);
            }
            calibrationVoltage1 = calibrationVoltagesPos1.Average();
            calibrationVoltage2 = calibrationVoltagesPos2.Average();

            firstCalibrationDone = true;

            if (calibrationVoltage1 - calibrationVoltage2 == 0)
            {
                Debug.Log("Calibration voltage 1 is equal to calibration voltage 2. Try to calibrate again");
                StartCoroutine(ChangeDynamicButtonColorError());
            }
            else
            {
                secondCalibrationDone = true;
                StartCoroutine(ChangeDynamicButtonColor());
            }
            
        }
    }

    IEnumerator CalibratePos1()
    {
        startOfProgram = false;
        measureWithOldCalibrationValues = false;
        if (calibrated == false && bodyPartToggle.isOn)
        {

            calibrationAngle1 = calibrationSlider1.value;
            float endTime = Time.time + calibrationTime;
            List<double>[] calibrationValuesLists = new List<double>[numberOfHasels]; // array of seven lists
            float[] calibrationVoltagesPos1 = new float[numberOfHasels]; // array of 3 calibration voltages
            for (int i = 0; i < numberOfHasels; i++)
            {
                calibrationValuesLists[i] = new List<double>(); // initialize each list
            }

            while (Time.time < endTime)
            {
               // Debug.Log("Running concurrent task...");

                // Get filtered values                
                for(int i = 0; i < numberOfHasels; i++)
                {
                    double filteredValue = dataProcessor.GetFilteredValue(i+firstHaselIndex-1);
                    calibrationValuesLists[i].Add(filteredValue); // add the value to the corresponding list
                }
                

                yield return null;
            }
            // calculate the calibration voltages for each list
            for (int i = 0; i < numberOfHasels; i++)
            {
                calibrationVoltagesPos1[i] = Convert.ToSingle(calibrationValuesLists[i].Average());
                //Debug.Log("Calibration voltage " + (i + 1) + ": " + calibrationVoltagesPos1[i]);
            }
            calibrationVoltage1 = calibrationVoltagesPos1.Average();
            Debug.Log("Calibration Voltage Pos 1: " + calibrationVoltage1);
            Debug.Log("Calibration Pos1 finished.");

            firstCalibrationDone = true;
            StartCoroutine(ChangeButtonColor1());
        }
    }
  
    IEnumerator CalibratePos2()
    {
        if (calibrated == false && bodyPartToggle.isOn)
        {

            calibrationAngle2 = calibrationSlider2.value;
            float endTime = Time.time + calibrationTime;
            List<double>[] calibrationValuesLists = new List<double>[numberOfHasels]; // array of seven lists
            float[] calibrationVoltagesPos2 = new float[numberOfHasels]; // array of seven calibration voltages
            for (int i = 0; i < numberOfHasels; i++)
            {
                calibrationValuesLists[i] = new List<double>(); // initialize each list
            }

            while (Time.time < endTime)
            {
                Debug.Log("Running concurrent task...");

                // Get filtered values                
                for (int i = 0; i < numberOfHasels; i++)
                {
                    double filteredValue = dataProcessor.GetFilteredValue(i+firstHaselIndex-1);
                    calibrationValuesLists[i].Add(filteredValue); // add the value to the corresponding list
                }

                yield return null;
            }
            for (int i = 0; i < numberOfHasels; i++)
            {
                calibrationVoltagesPos2[i] = Convert.ToSingle(calibrationValuesLists[i].Average());
                //Debug.Log("Calibration voltage " + (i + 1) + ": " + calibrationVoltagesPos1[i]);
            }
            calibrationVoltage2 = calibrationVoltagesPos2.Average();
            Debug.Log("Calibration Voltage Pos 2: " + calibrationVoltage2);
            Debug.Log("Calibration Pos2 finished.");

            if (calibrationVoltage1 - calibrationVoltage2 == 0)
            {
                Debug.Log("Calibration voltage 1 is equal to calibration voltage 2. Try to calibrate again");
                StartCoroutine(ChangeButtonColorError());
            }
            else
            {
                secondCalibrationDone = true;
                StartCoroutine(ChangeButtonColor2());
            }
            
        }

    }

    IEnumerator ChangeDynamicButtonColorError()
    {
        calibrateButton3.GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(1f); // Wait for 1 second
        calibrateButton3.GetComponent<Image>().color = Color.white; // Change the color back to white

    }

    IEnumerator ChangeDynamicButtonColor()
    {
        calibrateButton3.GetComponent<Image>().color = Color.green;
        yield return new WaitForSeconds(0.2f); // Wait for 0.1 second
        calibrateButton3.GetComponent<Image>().color = Color.white; // Change the color back to white

        List<double> calibrationList = new List<double>();
        calibrationList.Add(calibrationVoltage1);
        calibrationList.Add(calibrationVoltage2);
        calibrationList.Add(calibrationAngle1);
        calibrationList.Add(calibrationAngle2);
        using (StreamWriter writer = new StreamWriter($@"C:\tmp\values_calibration_{firstHaselIndex}.txt", false))
        {
            string fileContent = string.Join(",", calibrationList);
            writer.Write(fileContent);
        }
    }

    IEnumerator ChangeButtonColor1()
    {
        calibrateButton1.GetComponent<Image>().color = Color.green;
        yield return new WaitForSeconds(0.2f); // Wait for 0.1 second
        calibrateButton1.GetComponent<Image>().color = Color.white; // Change the color back to white
    }

    IEnumerator ChangeButtonColor2()
    {
        calibrateButton2.GetComponent<Image>().color = Color.green;
        yield return new WaitForSeconds(0.2f); // Wait for 0.1 second
        calibrateButton2.GetComponent<Image>().color = Color.white; // Change the color back to white

        List<double> calibrationList = new List<double>();
        calibrationList.Add(calibrationVoltage1);
        calibrationList.Add(calibrationVoltage2);
        calibrationList.Add(calibrationAngle1);
        calibrationList.Add(calibrationAngle2);
        using (StreamWriter writer = new StreamWriter($@"C:\tmp\values_calibration_{firstHaselIndex}.txt", false))
        {
            string fileContent = string.Join(",", calibrationList);
            writer.Write(fileContent);
        }
    }
    IEnumerator ChangeButtonColorError()
    {
        calibrateButton1.GetComponent<Image>().color = Color.red;
        calibrateButton2.GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(1f); // Wait for 1 second
        calibrateButton1.GetComponent<Image>().color = Color.white; // Change the color back to white
        calibrateButton2.GetComponent<Image>().color = Color.white;

    }

    IEnumerator ChangeApplicationButtonColor()
    {
        startApplicationButton.GetComponent<Image>().color = Color.yellow;
        yield return new WaitForSeconds(0.5f); // Wait for 0.1 second
        startApplicationButton.GetComponent<Image>().color = Color.white; // Change the color back to white
    }


    public void RotationUpdate(System.Single value)
    {
        if (calibrated == false && bodyPartToggle.isOn)
        {
            Vector3 to = new Vector3((value + AngleOffset) * RotationDirectionX, (value + AngleOffset) * RotationDirectionY, (value + AngleOffset) * RotationDirectionZ);

            transform.localEulerAngles = to;
        }

    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (selfsensingTesting == true)
        {
            float[] floatValues = new float[numberOfHasels];
            float vectorRotation = 0;

           
            //Get first 3 values
            for (int i = 0; i < numberOfHasels; i++)
            {
                floatValues[i] = (float)dataProcessor.GetFilteredValue(i+firstHaselIndex-1);
                vectorRotation += (floatValues[i] - voltageOffsetEstim) * voltagetoDegEstim;
            }
            vectorRotation /= numberOfHasels;
            
            Debug.Log(vectorRotation.ToString());
            Vector3 to = new Vector3(0, vectorRotation, 0);

            transform.localEulerAngles = to;
        }
        */
        if (bodyPartToggle.isOn && (calibrated == true || measureWithOldCalibrationValues == true))
        {
            float[] floatValues = new float[numberOfHasels];
            float vectorRotation = 0;
            //Get first 3 values
            for (int i = 0; i < numberOfHasels; i++)
            {
                floatValues[i] = (float)dataProcessor.GetFilteredValue(i+firstHaselIndex-1);
                vectorRotation += ((floatValues[i] - calibrationVoltage1) / (calibrationVoltage2 - calibrationVoltage1)) * (calibrationAngle2 - calibrationAngle1) + calibrationAngle1;
            }
            vectorRotation /= numberOfHasels;
            //Debug.Log(vectorRotation.ToString());
            Vector3 to = new Vector3((vectorRotation + AngleOffset) * RotationDirectionX , (vectorRotation + AngleOffset) * RotationDirectionY, (vectorRotation + AngleOffset) * RotationDirectionZ);

            transform.localEulerAngles = to;
            
        }

    }

}
