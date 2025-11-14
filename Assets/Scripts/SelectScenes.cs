using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectScenes : MonoBehaviour
{
    public Button loadScenes01;

    public Button loadScenea02;

    void Start()
    {
        loadScenes01.onClick.AddListener(Load01);
        loadScenea02.onClick.AddListener(Load02);
    }

    
    void Update()
    {
        
    }

    private void Load01()
    {
        SceneManager.LoadScene("Scene01");
    }

    private void Load02()
    {
        SceneManager.LoadScene("Scene02");
    }

}
