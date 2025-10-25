using UnityEngine;

public class dispararbolas : MonoBehaviour
{
    public Transform mano;
    public int time;
    public int force;
    private Vector3 direction;
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
        Destroy(gameObject, time);
        // Ensure it's a unit vector
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * Time.deltaTime * force;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2);
    }
}
