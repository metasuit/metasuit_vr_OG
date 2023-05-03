using UnityEngine;
using UnityEngine.UI;

public class ButtonColorChanger : MonoBehaviour
{
    public Color colorDefault;
    public Color colorActivated;
    Button button;
    public Button startApplicationButton;
    public bool isActivated = false;

    void Start()
    {
        button = GetComponent<Button>();
        
        /*
        GameObject otherGameObject = GameObject.Find("EndCalibrationButton"); // Replace "OtherGameObject" with the name of the GameObject that has the OtherScript attached to it.
        if (otherGameObject != null)
        {
            Button otherButton = otherGameObject.GetComponent<Button>();
        }
        else
        {
            Debug.Log("GameObject with name 'OtherGameObject' not found.");
        }
        */
        button.onClick.AddListener(ChangeColor);
        startApplicationButton.onClick.AddListener(ChangeColor2);
    }
    void ChangeColor2()
    {
        if (isActivated == true)
        {
            GetComponent<Image>().color = colorDefault;
            isActivated = false;
        }
       
    }


    void ChangeColor()
    {
        isActivated = !isActivated;
        if(isActivated == true)
        {
            GetComponent<Image>().color = colorActivated;
        }
        else
        {
            GetComponent<Image>().color = colorDefault;
        }
      
    }
}