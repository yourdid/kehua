using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CarLightController : MonoBehaviour
{
    public GameObject BigLight;

    public GameObject smallLight;

    public GameObject carLight;

    private bool isOpen;

   
    
    void Start()
    {
        isOpen = false;
       
    }

   
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.F1))
        {
            BigLight.SetActive(false);
            smallLight.SetActive(true);   
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            BigLight.SetActive(true);
            smallLight.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            BigLight.SetActive(false);
            smallLight.SetActive(false);
        }
        if (Input.GetKeyDown(KeyCode.F4))
        {
            if (isOpen == false)
            {
                carLight.SetActive(true);
                isOpen = true;
            }
            else
            {
                carLight.SetActive(false);
                isOpen = false;
            }
        }
    }
}
