using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject loadingText;
    

    public void PlayGame()
    {
        
        
        // if ad failed or loaded, doesn't matter, load the next scene
        SceneManager.LoadScene(1);
        loadingText.SetActive(true);
    }
}
