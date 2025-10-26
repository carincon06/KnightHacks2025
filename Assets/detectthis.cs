using UnityEngine;

public class detectthis : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("dragon"))
        {
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
    
}
