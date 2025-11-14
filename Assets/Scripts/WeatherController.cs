using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WeatherController : MonoBehaviour
{
    //public Button nightBtn;

    //public Button dayBtn;

    //public Button rainBtn;

    //public Button snowBtn;

    //public Button Loadscene;

    public GameObject night;

    public GameObject day;

    public GameObject rain;

    private bool isRain;

    public GameObject snow;

    private bool isSnow;

    public GameObject Fog;

    private bool isFog;

    public Material NaightSky;

    public Material DaySky;

    private Color lightest;//中午颜⾊

   // public GameObject M01;

    //public GameObject M02;

   // public Texture[] materials01;

    //public Texture[] materials02;



    void Start()
    {
        isSnow = false;
        isRain = false;
        isFog = false;
        // nightBtn.onClick.AddListener(Night);
        //dayBtn.onClick.AddListener(Day);
        // rainBtn.onClick.AddListener(Rain);
        //snowBtn.onClick.AddListener(Snow);
        //Loadscene.onClick.AddListener(LaodRoad);
        lightest = new Color32(254, 254, 254, 1);

        
    }

   
    void Update()
    {
        //Y键晚上，U键白天，I键雪，O键雨，p键雾
        if (Input.GetKeyDown(KeyCode.Y))
        {
            Night();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Day();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            Snow();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Rain();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            FogColl();
        }
       if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(1);
        }
    }

    private void Night()
    {
        night.SetActive(true);
        day.SetActive(false);
        RenderSettings.skybox = NaightSky;

        RenderSettings.ambientLight = new Color32(80, 80, 80, 0);
        //M01.GetComponent<MeshRenderer>().material.mainTexture = materials01[0];
        //M02.GetComponent<MeshRenderer>().material.mainTexture = materials02[0];
        //M01.SetActive(false);
        //M02.SetActive(true);
    }

    private void Day()
    {
        night.SetActive(false);
        day.SetActive(true);
        RenderSettings.skybox = DaySky;
        RenderSettings.ambientLight = lightest;

        // M01.GetComponent<MeshRenderer>().material.mainTexture = materials01[1];
        // M02.GetComponent<MeshRenderer>().material.mainTexture = materials02[1];

        //M01.SetActive(true);
       //M02.SetActive(false);
    }

    private void Snow()
    {
        if (isSnow == false)
        {
            snow.SetActive(true);
            isSnow = true;
            RenderSettings.fog = true;
        }
        else
        {
            snow.SetActive(false);
            isSnow = false;
            RenderSettings.fog = false;
        }
        
    }

    private void Rain()
    {
        if (isRain == false)
        {
            rain.SetActive(true);
            isRain = true;
            RenderSettings.fog = true;
        }
        else
        {
            rain.SetActive(false);
            isRain = false;
            RenderSettings.fog = false;
        }
    }

    private void LaodRoad()
    {
        SceneManager.LoadScene("Start");
    }

    private void FogColl()
    {
        if (isFog == false)
        {
            Fog.SetActive(true);
            isFog = true;
            RenderSettings.fog = true;
        }
        else
        {
            Fog.SetActive(false);
            isFog = false;
            RenderSettings.fog = false;
        }
    }
}
