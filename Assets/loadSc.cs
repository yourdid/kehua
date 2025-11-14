using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class loadSc : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void exit()
    {
        Application.Quit();
    }
    public void loadSence(int i)
    {
        SceneManager.LoadScene(i);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
