using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class SplashScreen : MonoBehaviour
{
    public GameObject mainMenu;
    private VideoPlayer _videoPlayer;
    
    void Start()
    {
        _videoPlayer = GetComponent<VideoPlayer>();
        _videoPlayer.Play();
    }

    void Update()
    {
        if (!_videoPlayer.isPlaying && _videoPlayer.isPrepared)
        {
            mainMenu.SetActive(true);
            Destroy(this);
        }
    }
}
