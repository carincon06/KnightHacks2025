using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
public class magicmanMovement : MonoBehaviour
{
    public int speed;
    public float frequency;
    public int amplitude;
    private Vector3 startPosition;
    private int direction = 1;
    private float elapsedTime = 0f;
    public float delay;
    public GameObject bola;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(direction * speed * Time.deltaTime, Mathf.Sin(Time.time * frequency) * amplitude * Time.deltaTime, 0);

        if (transform.position.x < -30)
        {
            direction *= -1;
        }
        else if(transform.position.x > 50)
        {
            direction *= -1;
        }

    }
   
}
