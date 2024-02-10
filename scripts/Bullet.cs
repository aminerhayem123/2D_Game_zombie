using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Called when the bullet collides with another Collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider belongs to an enemy
        if (other.CompareTag("enemy"))
        {
            // Retrieve the Enemy script from the collided GameObject
            Enemy enemy = other.GetComponent<Enemy>();

            // Check if the enemy script is not null
            if (enemy != null)
            {
                // Deal damage to the enemy (you can adjust the damage value)
                enemy.TakeDamage(1);

                // Destroy the bullet after hitting the enemy
                Destroy(gameObject);
            }
        }
    }
}
