using UnityEngine;

public class FireballBehavior : MonoBehaviour
{
    // Assign your explosion particle system to this in the Inspector
    public GameObject explosionEffectPrefab; // Changed to GameObject prefab
    
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
        // Spawn explosion at impact point
        SpawnExplosion(collision.contacts[0].point);

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

    void SpawnExplosion(Vector3 position)
    {
        if (explosionEffectPrefab != null)
        {
            // Instantiate the explosion at the impact point
            GameObject explosion = Instantiate(explosionEffectPrefab, position, Quaternion.identity);
            
            // Get the particle system
            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
            
            if (ps != null)
            {
                // Make sure it's NOT set to loop
                var main = ps.main;
                main.loop = false;
                
                // Play the effect
                ps.Play();
                
                // Destroy after the particle system is done
                Destroy(explosion, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            else
            {
                // If no particle system, destroy after 2 seconds
                Destroy(explosion, 2f);
            }
        }
    }
}