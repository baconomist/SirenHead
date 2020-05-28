using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public bool manualRestart = false;
    
    private void OnValidate()
    {
        if(manualRestart)
            Restart();
    }

    public static void OnGameOver()
    {
        if (Ads.IsLoaded())
            Ads.ShowInterstitial(Restart);
        else
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
}