using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CalibrationSliderController : MonoBehaviour
{
    public TextMesh valueText;
    // Start is called before the first frame update
    public void OnSliderChanged(float value)
    {
        valueText.text = value.ToString();
    }
}
