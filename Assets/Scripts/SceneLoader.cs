using UnityEngine;
using UnityEngine.SceneManagement; // 1. You MUST import this to use SceneManager

public class SceneLoader : MonoBehaviour
{
    // 2. This is the public function the button will call
    public void LoadScene(string sceneName)
    {
        // 3. This line does the work
        SceneManager.LoadScene(sceneName);
    }
}