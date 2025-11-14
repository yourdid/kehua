using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tongguo1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            aqjsManager.instance.pass();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
