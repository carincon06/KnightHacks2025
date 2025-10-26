using UnityEngine;

public class FireballBehavior : MonoBehaviour
{
    [Header("Fireball Settings")]
    public float lifetime = 5f;
    public float damageAmount = 10f;

    void Start()
    {
        // Destroy the fireball after X seconds if it doesn't hit anything
        Destroy(gameObject, lifetime);
    }

    // This runs when the fireball collides with another solid object
    void OnCollisionEnter(Collision collision)
    {
        // Optional: Check if we hit an enemy
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Just destroy the enemy for now
            // TODO: Add proper health system later
            Destroy(collision.gameObject);
        }

        // Destroy the fireball
        Destroy(gameObject);
    }
}