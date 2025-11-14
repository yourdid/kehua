using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class videoManager : MonoBehaviour
{
    public GameObject video;
    public VideoPlayer videoplay;
    public VideoClip[] clip;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void playFormIndex(int i)
    {
        if (clip[i]!=null)
        {
            video.SetActive(true);
            videoplay.clip=clip[i];
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
