using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class zhanghaoming : MonoBehaviour
{
    public Text zhanghaotext;
    // Start is called before the first frame update
    void Start()
    {
        zhanghaotext = GetComponent<Text>();
        if(LoginClass.myUsername!=null)
        {
            zhanghaotext.text = LoginClass.myUsername;
        }
        else
        {
            zhanghaotext.text = "сн©м";
        }    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
