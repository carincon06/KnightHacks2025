using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private int howmanyenemies = 7;
    public string nextScene;

    void Update()
    {
        // Count remaining enemies
        if (GameObject.FindGameObjectsWithTag("dragon").Length == 0)
        {
            // All enemies are dead, load next scene
            SceneManager.LoadScene(nextScene);
        }
    }
}
