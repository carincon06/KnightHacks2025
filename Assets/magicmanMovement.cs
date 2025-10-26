using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
public class magicmanMovement : MonoBehaviour
{
    public float speed;
    public float frequency;
    public int amplitude;
    private Vector3 startPosition;
    private int direction = 1;
    private float elapsedTime = 0f;
    public float delay;
    public GameObject bola;
    public float startDelay;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(Mathf.Sin(Time.time * frequency) * amplitude * Time.deltaTime, direction * speed * Time.deltaTime, 0);

        if (transform.position.y < 5)
        {
            direction *= -1;
        }
        else if(transform.position.y > 8)
        {
            direction *= -1;
        }

    }
   
}
