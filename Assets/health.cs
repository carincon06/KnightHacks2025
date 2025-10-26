using UnityEngine;
using UnityEngine.SceneManagement;

public class health : MonoBehaviour
{
    private int healthLevel = 5;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("fireballenemy"))
        {
            healthLevel--;
            Debug.Log(healthLevel.ToString());
        }
        if(healthLevel == 0)
        {
            SceneManager.LoadScene("YouLoseScreen");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
