using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
public class zuigaofen : MonoBehaviour
{
    public Text zuigaofentext;
    // Start is called before the first frame update
    void Start()
    {
        zuigaofentext = GetComponent<Text>();
        if (LoginClass.myUsername != null)
        {
            zuigaofentext.text = PlayerPrefs.GetInt(LoginClass.myUsername + "score", 0).ToString();
        }
        else
        {
            zuigaofentext.text = "0";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
