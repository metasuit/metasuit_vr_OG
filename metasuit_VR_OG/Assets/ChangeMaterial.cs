using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class ChangeMaterial : MonoBehaviour
{
    public InputActionProperty showButton;
    public Renderer objectRenderer;
    public Material TucanMaterial;
    public Material ParrotMaterial;
    public Material CockatielMaterial;
    public Material KingFisherMaterial;
    public Material[] materials = new Material[4];

    public CallBird callBirdScript;

    // Start is called before the first frame update
    void Start()
    {
        materials[0] = TucanMaterial;
        materials[1] = ParrotMaterial;
        materials[2] = CockatielMaterial;
        materials[3] = KingFisherMaterial;
     
        objectRenderer = GetComponent<Renderer>();
        objectRenderer.enabled = false;
    }

}
