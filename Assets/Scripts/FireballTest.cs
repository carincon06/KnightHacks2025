using UnityEngine;

// TEMPORARY: Test fireball without hand tracking
// Attach this to your Main Camera, press SPACE to shoot
public class FireballTest : MonoBehaviour
{
    public GameObject fireballPrefab;
    public float fireballSpeed = 20f;

    void Update()
    {
        // Press SPACE to shoot a test fireball
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShootTestFireball();
        }
    }

    void ShootTestFireball()
    {
        if (fireballPrefab == null)
        {
            Debug.LogError("Fireball prefab not assigned!");
            return;
        }

        // Spawn in front of camera
        Vector3 spawnPos = transform.position + transform.forward * 2f;
        GameObject fireball = Instantiate(fireballPrefab, spawnPos, Quaternion.identity);

        // Add velocity
        Rigidbody rb = fireball.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
            rb.linearVelocity = transform.forward * fireballSpeed;
            Debug.Log($"TEST: Fireball shot! Velocity: {rb.linearVelocity}");
        }
        else
        {
            Debug.LogError("TEST: Fireball missing Rigidbody!");
        }
    }
}