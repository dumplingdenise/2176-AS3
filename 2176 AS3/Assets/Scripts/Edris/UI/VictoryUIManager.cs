using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUIManager : MonoBehaviour
{
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");         // if the loadscene changes, please change the scene name here "".
    }
}








