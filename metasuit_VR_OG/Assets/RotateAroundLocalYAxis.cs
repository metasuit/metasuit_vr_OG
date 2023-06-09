using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Data;
using System.Linq;
using System.IO;
using System.Text;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearRegression;
using Button = UnityEngine.UI.Button;
using MathNet.Numerics.LinearAlgebra.Double;

public class RotateAroundLocalYAxis : MonoBehaviour
{
    public bool calibrationVisualizationToggle = false;
    public bool canWrite = false;
    public bool calibrated = false;
    public bool dynamicCalibrationDone = false;
    bool startOfProgram = true;
    bool measureWithOldCalibrationValues = false;
    private Vector<double> coefficients = Vector<double>.Build.Dense(4);
    public RotateAroundLocalYAxis otherBodyScript;
    //public float amplitude = 45.0f;
    //public float frequency = 1.0f;
    //public float offset = 45.0f;
    public int bodyPartIndex;
    public int RotationDirectionX = 0;
    public int RotationDirectionY = 1;
    public int RotationDirectionZ = 0;
    public int AngleOffset = 0;
    public int MaxRotationAngle = 180;
    public int MinRotationAngle = -180;
    public float calibrationTimeMove = 6;
    public int numberOfHasels;
    public int firstHaselIndex;
    public Button calibrateButton3;
    public Slider calibrationSlider1;
    public Slider calibrationSlider2;
    public Button startCalibrationButton;
    public Button startApplicationButton;
    public DataProcessor dataProcessor;
    public Toggle bodyPartToggle;

    private void Start()
    {
        // Inititalize calibration wtih last calibrated values
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
                // Get calibration coefficients of linear regression fit
                string[] values = line.Split(',');
                coefficients[0] = float.Parse(values[0]);
                coefficients[1] = float.Parse(values[1]);
                coefficients[2] = float.Parse(values[2]);
                coefficients[3] = float.Parse(values[3]);
            }
        }
        calibrateButton3.onClick.AddListener(ButtonClick3);
        startCalibrationButton.onClick.AddListener(StartCalibration);
        startApplicationButton.onClick.AddListener(EndCalibration);   
    }
    void StartCalibration()
    {
        if(bodyPartToggle.isOn)
        {
            calibrated = false;
            dynamicCalibrationDone = false;
            startOfProgram = false;
            measureWithOldCalibrationValues = false;
        }
    }
    void EndCalibration()
    {
        if (bodyPartToggle.isOn)
        {
            if (!canWrite) // if body part which doesn't write -> get newest calibration values
            {
                
                // Inititalize calibration wtih last calibrated values
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
                        // Get calibration coefficients of linear regression fit
                        string[] values = line.Split(',');
                        coefficients[0] = float.Parse(values[0]);
                        coefficients[1] = float.Parse(values[1]);
                        coefficients[2] = float.Parse(values[2]);
                        coefficients[3] = float.Parse(values[3]);
                    }
                }
            }
            else if (dynamicCalibrationDone)
            {
                calibrated = true;
                otherBodyScript.calibrated = true;
            }
            else if (startOfProgram == true)
            {
                // if started without calibrating, use old calibration values
                measureWithOldCalibrationValues = true;
                otherBodyScript.measureWithOldCalibrationValues = true;
                StartCoroutine(ChangeApplicationButtonColor());
            }
            
            else Debug.Log("Calibration did not work");
        }
        

    }
    void ButtonClick3()
    {
        StartCoroutine(CalibrateDynamic());
    }

    IEnumerator CalibrateDynamic()
    {
        
        if (calibrated == false && bodyPartToggle.isOn && canWrite)
        {
            List<double> linregMeas = new List<double>();
            List<double> linregAngles = new List<double>();

            startOfProgram = false;
            measureWithOldCalibrationValues = false;

            float endTime = Time.time + calibrationTimeMove;
            float startTime = Time.time;
            // get impedance and angle measurements for calibration
            StartCoroutine(ChangeDynamicButtonColorCalibrating());
            while (Time.time < endTime)
            {
                //float sineWaveValue = (float)(Math.Sin((Time.time-startTime) * frequency - Math.PI / 2) * amplitude + offset);

                float imp_meas = 0;
                float angle_meas = 0;
                for (int i = 0; i < numberOfHasels; i++)
                {                                                                                                                                                        
                    imp_meas += (float)dataProcessor.GetImpedance(i + firstHaselIndex - 1);                  
                }
                imp_meas /= numberOfHasels;

                angle_meas = (float)dataProcessor.GetAngle(bodyPartIndex);
                // Calculate the new rotation around the Y axis using the sine wave value
                // Vector3 newRotation = new Vector3((sineWaveValue + AngleOffset) * RotationDirectionX, (sineWaveValue + AngleOffset) * RotationDirectionY, (sineWaveValue + AngleOffset) * RotationDirectionZ);

                // Set the new rotation of the game object in Euler angles
                //transform.localEulerAngles = newRotation;
         

                Debug.Log(imp_meas);

                linregMeas.Add(imp_meas);
                linregAngles.Add(angle_meas);

                yield return null;
            }

           

            //save measurements if toggled
            if (calibrationVisualizationToggle)
            {
                using (StreamWriter writer = new StreamWriter($@"C:\tmp\imp_meas_{firstHaselIndex}.txt", false))
                {
                    string fileContent = string.Join(",", linregMeas);
                    writer.Write(fileContent);
                }
                using (StreamWriter writer = new StreamWriter($@"C:\tmp\angles_{firstHaselIndex}.txt", false))
                {
                    string fileContent = string.Join(",", linregAngles);
                    writer.Write(fileContent);

                }
            }

            /*
            // Remove first few samples to account for inaccuracy at start of movement
            linregMeas.RemoveRange(0, 40);
            linregAngles.RemoveRange(0, 40);
            */

            // Convert the data to array
            double[] MeasArray = linregMeas.ToArray();
            double[] AnglesArray = linregAngles.ToArray();

            // Define the model function
            double[][] A = MeasArray.Select(x => new[] {Math.Pow(x, 3), Math.Pow(x, 2), x, 1.0 }).ToArray();
            double[] y = AnglesArray;

            // Convert the arrays to matrices
            Matrix<double> X = DenseMatrix.OfRowArrays(A);
            Vector<double> Y = DenseVector.OfArray(y);

            //multiple regression

            try
            {
                coefficients = MultipleRegression.NormalEquations(X, Y);
            }
            catch (Exception ex) 
            { 
                Debug.LogError(ex);
                StartCoroutine(ChangeDynamicButtonColorError());
            }
          


            //Debugging, write coefficients and arrays into files to visualize relationship
            /*
            using (StreamWriter writer = new StreamWriter(@"C:\tmp\lr_coefficients.txt", false))
            {
                string fileContent = string.Join(",", coefficients);
                writer.Write(fileContent);
            }
            */
            Debug.Log("measurements" + linregMeas.Count().ToString());
            Debug.Log("angles" + linregAngles.Count().ToString());
            
            dynamicCalibrationDone = true;
            StartCoroutine(ChangeDynamicButtonColor());
        }
    }

    IEnumerator ChangeDynamicButtonColorError()
    {
        calibrateButton3.GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(1f); // Wait for 1 second
        calibrateButton3.GetComponent<Image>().color = Color.white; // Change the color back to white

    }
    IEnumerator ChangeDynamicButtonColorCalibrating()
    {
        calibrateButton3.GetComponent<Image>().color = Color.green;
        yield return new WaitForSeconds(0.1f);
    }
    IEnumerator ChangeDynamicButtonColor()
    {
        //calibrateButton3.GetComponent<Image>().color = Color.green;
        yield return new WaitForSeconds(0.2f); // Wait for 0.1 second
        calibrateButton3.GetComponent<Image>().color = Color.white; // Change the color back to white

        //save values of last calibration regression model
        List<double> calibrationList = new List<double>();
        calibrationList.Add(coefficients[0]);
        calibrationList.Add(coefficients[1]);
        calibrationList.Add(coefficients[2]);
        calibrationList.Add(coefficients[3]);
        using (StreamWriter writer = new StreamWriter($@"C:\tmp\values_calibration_{firstHaselIndex}.txt", false))
        {
            string fileContent = string.Join(",", calibrationList);
            writer.Write(fileContent);
        }
    }

    IEnumerator ChangeApplicationButtonColor()
    {
        startApplicationButton.GetComponent<Image>().color = Color.yellow;
        yield return new WaitForSeconds(0.5f); // Wait for 0.1 second
        startApplicationButton.GetComponent<Image>().color = Color.white; // Change the color back to white
    }


    // Update is called once per frame
    void Update()
    {

        //float angle_meas = (float)dataProcessor.GetAngle(bodyPartIndex);
        //Vector3 toforvideo = new Vector3((angle_meas + AngleOffset) * RotationDirectionX, (angle_meas + AngleOffset) * RotationDirectionY, (angle_meas + AngleOffset) * RotationDirectionZ);
       // Debug.Log(toforvideo);

        //transform.localEulerAngles = toforvideo;

        // Check if body part is activated and calibrated
        if (bodyPartToggle.isOn && (calibrated == true || measureWithOldCalibrationValues == true))
        {
            double[] floatValues = new double[numberOfHasels];
            double vectorRotation = 0;
            //Get first 3 values
            for (int i = 0; i < numberOfHasels; i++)
            {
                // Get values from dataprocessor object
                floatValues[i] = (float)dataProcessor.GetImpedance(i+firstHaselIndex-1);
                vectorRotation += coefficients[0] * floatValues[i] * floatValues[i] * floatValues[i] + coefficients[1] * floatValues[i] * floatValues[i] + coefficients[2] * floatValues[i] + coefficients[3];
            }
            vectorRotation /= numberOfHasels;
            //Restrict Rotation
            if(vectorRotation + AngleOffset > MaxRotationAngle) { vectorRotation = MaxRotationAngle - AngleOffset; }
            else if(vectorRotation + AngleOffset < MinRotationAngle) { vectorRotation = MinRotationAngle - AngleOffset; }

            Vector3 to = new Vector3(((float)vectorRotation + AngleOffset) * RotationDirectionX , ((float)vectorRotation + AngleOffset) * RotationDirectionY, ((float)vectorRotation + AngleOffset) * RotationDirectionZ);
            transform.localEulerAngles = to;          
        }
    }
}
