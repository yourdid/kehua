using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.Diagnostics;


public class LoginClass : MonoBehaviour
{
    //进入前变量
    public InputField username, password;
    public Text reminderText, reminderText2;
    public int errorsNum;
    public Button loginButton;


    public InputField Zusername, Zpassword, Zqrpassword;
    //  public GameObject hallSetUI, loginUI;
    //进入后变量
    public static string myUsername;
    
    public void Awake()
    {

        DontDestroyOnLoad(gameObject);
    }

    public void Register()
    {
        if (PlayerPrefs.GetString(Zusername.text) == "")
        {
            if (Zqrpassword.text == Zpassword.text)
            {
                PlayerPrefs.SetString(Zusername.text, Zusername.text);
                PlayerPrefs.SetString(Zusername.text + "password", Zpassword.text);
                reminderText2.text = "注册成功！";
            }
            else
            {
                reminderText2.text = "两次输入的密码不一样";
            }

        }
        else
        {
            reminderText2.text = "用户已存在";
        }

    }
    private void Recovery()
    {
        loginButton.interactable = true;
    }
    public void Login()
    {
        if (PlayerPrefs.GetString(username.text) != "")
        {
            if (PlayerPrefs.GetString(username.text + "password") == password.text)
            {
                reminderText.text = "登录成功";

                myUsername = username.text;
                // hallSetUI.SetActive(true);
                //  loginUI.SetActive(false);
                SceneManager.LoadScene(1);
            }
            else
            {
                reminderText.text = "密码错误";
                errorsNum++;
                if (errorsNum >= 3)
                {
                    reminderText.text = "连续错误3次，请30秒后再试！";
                    loginButton.interactable = false;
                    Invoke("Recovery", 5);
                    errorsNum = 0;
                }
            }
        }
        else
        {
            reminderText.text = "账号不存在";
        }
    }

    private string GetDebuggerDisplay()
    {
        return ToString();
    }
    public void ClearAll()
    {
        PlayerPrefs.DeleteAll();
    }
}
