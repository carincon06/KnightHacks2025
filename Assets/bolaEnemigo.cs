using UnityEngine;
using System.Collections;
public class bolaEnemigo : MonoBehaviour
{    
    public int time;
    public float force;
    private Vector3 direction;
    public GameObject bola;
    public Transform shootPoint;
    public float shootDelay = 2f;
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
        Destroy(gameObject, time);
    }

    

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
    void Start()
    {
        StartCoroutine(ShootLoop());
    }

    IEnumerator ShootLoop()
    {
        while (true)
        {
            Shoot();
            yield return new WaitForSeconds(shootDelay);
        }
    }

    void Shoot()
    {
        GameObject ballInstance = Instantiate(bola, shootPoint.position, shootPoint.rotation);
        dispararbolas projectile = ballInstance.GetComponent<dispararbolas>();
        projectile.time = 2;
        projectile.SetDirection(shootPoint.forward);

    }
}
