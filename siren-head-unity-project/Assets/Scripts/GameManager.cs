
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static void OnPlayerDied()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public static void OnGameFinished()
    {
    }
}
