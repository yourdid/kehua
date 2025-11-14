using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class datimanager : MonoBehaviour
{
    public static datimanager instance;
    public int score;
    public Text scoretext;
    public float timer;
    public Text timerText;
    public GameObject huidazq;
    public GameObject huidacuowu;
    public GameObject jieshu;
    public bool jishi;

    public GameObject[] timu;
    public int index;
    public bool[] b;
    public void IncScore()
    {
        score += 5;
        scoretext.text = score.ToString();
        huidazq.SetActive(true);
        GetShow();
    }
    public void cuowu()
    {
        huidacuowu.SetActive(true);
        GetShow();
    }
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        
    }
    public void jiesu()
    {
        if(LoginClass.myUsername!=null)
        {
            int i = PlayerPrefs.GetInt(LoginClass.myUsername + "score", 0);
            if(i<=score)
            {
                PlayerPrefs.SetInt(LoginClass.myUsername + "score", score);
            }
        }


        jieshu.SetActive(true);
        jishi = false;


    }
    public void GetShow()
    {
        jishi = true;
        index++;
        if (index > timu.Length)
        {
            jiesu();
            return;
        }
        for (int i = 0; i < timu.Length; i++)
        {
            int r = Random.Range(0, timu.Length);
            if (b[r] == false)
            {
                timu[r].SetActive(true);
                b[r] = true;
                return;
            }
            else
            {
                i--;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(jishi)
        {
            timer += Time.deltaTime;
            timerText.text = timer.ToString("0.0");
        }
    }
}
