using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject loadingText;
    
    private void Start()
    {
        Ads.ShowBanner();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(1);
        loadingText.SetActive(true);
    }
}
