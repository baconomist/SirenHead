using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public bool manualRestart = false;
    public GameObject pauseMenu;

    private void Awake()
    {
        _instance = this;
    }

    private void OnValidate()
    {
        if(manualRestart)
            Restart();
    }

    public static void OnGameOver()
    {
        Restart();
    }

    public static void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void OnGameFinished()
    {
        FindObjectOfType<Player>().GetComponentInChildren<CameraFade>().FadeCurve = AnimationCurve.Linear(0, 0, 3, 3);
        FindObjectOfType<Player>().GetComponentInChildren<CameraFade>().RedoFade();
        FindObjectOfType<GameManager>().Invoke("ToMainMenu", 3f);
    }

    public void ToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void OnPauseUnpause()
    {
        if (Time.timeScale < 1)
        {
            Time.timeScale = 1;
            HidePauseMenu();
        }
        else
        {
            Time.timeScale = 0;
            ShowPauseMenu();
        }
    }

    public void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
    }
    
    public void HidePauseMenu()
    {
        pauseMenu.SetActive(false);
    }
}