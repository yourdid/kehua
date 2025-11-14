using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dati : MonoBehaviour
{
    public bool a;
    public bool b;
    public bool c;


    public void AA()
    {
        if(a)
        {
            datimanager.instance.IncScore();
        }
        else
        {
            datimanager.instance.cuowu();
        }
        gameObject.SetActive(false);

    }
    public void BB()
    {
        if (b)
        {
            datimanager.instance.IncScore();
        }
        else
        {
            datimanager.instance.cuowu();
        }
        gameObject.SetActive(false);

    }
    public void CC()
    {
        if (c)
        {
            datimanager.instance.IncScore();
        }
        else
        {
            datimanager.instance.cuowu();
        }
        gameObject.SetActive(false);

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
