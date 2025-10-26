using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Loads your main game scene
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("MeadowDemo");
    }

    // Goes back to the main menu
    public void LoadMainMenu()
    {
        Debug.Log("LoadMainMenu pressed");
        SceneManager.LoadSceneAsync("MainMenu");
    }

    // Loads the "You Win" scene
    public void LoadWinScreen()
    {
        SceneManager.LoadSceneAsync("YouWinScreen");
    }

    // Loads the "You Lose" scene
    public void LoadLoseScreen()
    {
        SceneManager.LoadSceneAsync("YouLoseScreen");
    }

}
