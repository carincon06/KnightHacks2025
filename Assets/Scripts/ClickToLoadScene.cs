using UnityEngine;
using UnityEngine.SceneManagement;

public class ClickToLoadScene : MonoBehaviour
{
    public string sceneNameToLoad = "YourSceneName";
    
    void Update()
    {
        // Detect mouse click or touch
        if (Input.GetMouseButtonDown(0)) // 0 = left mouse button (also works for touch)
        {
            SceneManager.LoadScene(sceneNameToLoad);
        }
    }
}