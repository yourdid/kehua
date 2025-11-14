using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class aqjsManager : MonoBehaviour
{
    public static aqjsManager instance;
    public GameObject tongguo1;
    public GameObject shibai1;
    public bool isshibai;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        Time.timeScale = 0;
        
    }
      public void gamestart()
    {
        Time.timeScale = 1;
        
    }
    public void pass()
    {
        if (isshibai)
            return;
        tongguo1.SetActive(true);
    }
    public  void zhuangren()
    {
        shibai1.SetActive(true);
        isshibai = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(1);
        }
    }
}
