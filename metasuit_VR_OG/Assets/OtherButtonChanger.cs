using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OtherButtonChanger : MonoBehaviour
{
    Button button;
    private ButtonColorChanger otherScript;

    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
       // button.onClick.AddListener(ChangeColor);
    }
    void ChangeColor()
    {
        Debug.Log("button pressed");
 
        GameObject otherGameObject = GameObject.Find("StartCalibrationButton"); // Replace "OtherGameObject" with the name of the GameObject that has the OtherScript attached to it.
        if (otherGameObject != null)
        {
            Button otherButton = otherGameObject.GetComponent<Button>();
            Color newColor = Color.red; // Example color
            ColorBlock colorBlock = otherButton.colors;
            colorBlock.normalColor = newColor;
            otherButton.colors = colorBlock;
        }
        else
        {
            Debug.Log("GameObject with name 'OtherGameObject' not found.");
        }


    }
}
