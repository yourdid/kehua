using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xingren : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Player")
        {
            aqjsManager.instance.zhuangren();
        }
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
