using UnityEngine;
using UnityEngine.SceneManagement;

public class enemycounter : MonoBehaviour
{
    private int howmanyenemies = 7;
    public string nextOne;
    public void EnemyDied()
    {
        howmanyenemies--;
        if (howmanyenemies == 0)
        {
            SceneManager.LoadScene(nextOne);
        }
    }
}
