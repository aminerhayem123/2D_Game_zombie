using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float speed = 3f; // Enemy movement speed
    public Transform player; // Reference to the player
    public GameObject enemyPrefab; // Enemy prefab for respawning

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool hasIncreasedSpeed = false; // Flag to track speed increase

    void Start()
    {
        // Get the SpriteRenderer component attached to the same GameObject
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player != null)
        {
            // Move the enemy towards the player
            Vector2 direction = player.position - transform.position;
            direction.Normalize();
            transform.Translate(direction * speed * Time.deltaTime);

            // Flip the enemy sprite based on the movement direction
            FlipSprite(direction.x);

            // Check if night animator is active and speed has not been increased yet
            if (animator.GetBool("night") && !hasIncreasedSpeed)
            {
                // Increase speed by 1 when night animator is active
                speed += 1f * Time.deltaTime;
                hasIncreasedSpeed = true; // Set the flag to true to indicate speed increase
            }
        }
    }

    void FlipSprite(float directionX)
    {
        // Flip the sprite based on the direction (left or right)
        if (directionX < 0)
        {
            spriteRenderer.flipX = true; // Face left
        }
        else if (directionX > 0)
        {
            spriteRenderer.flipX = false; // Face right
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("caracter"))  // Change the tag to "Player"
        {
            // Do something when the enemy collides with the player (e.g., deal damage)
            Debug.Log("Player hit by enemy!");

            // You can add additional logic here for dealing damage to the player
        }
    }
}
