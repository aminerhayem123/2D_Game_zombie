using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 1;
    private Animator animator;
    private GameObject player;
    private lightController lightController;

    void Start()
    {
        // Get the Animator component attached to the same GameObject
        animator = GetComponent<Animator>();

        // Find the player by tag
        player = GameObject.FindGameObjectWithTag("caracter");

        // Find the lightController script in the scene
        lightController = FindObjectOfType<lightController>();
    }

    void Update()
    {
        // Check if the red light is on and the player is within danger range
        if (lightController != null && lightController.IsRedLightOn() && IsPlayerWithinDangerRange())
        {
            // Move towards the player (you can customize this based on your movement logic)
            MoveTowardsPlayer();
        }
    }

    // Called when the enemy is damaged
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            // Call a method for any additional logic before destroying the enemy
            OnDeath();
        }
    }

    // Called when the enemy is destroyed
    void OnDeath()
    {
        // Trigger the death animation
        animator.SetTrigger("die");

        // Add any additional logic or effects when the enemy is destroyed
        Destroy(gameObject);
    }

    // Move towards the player
    void MoveTowardsPlayer()
    {
        // Assuming a simple movement towards the player (you can customize this based on your movement logic)
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, Time.deltaTime);
    }

    // Check if the player is within danger range
    bool IsPlayerWithinDangerRange()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        return distance <= lightController.GetDangerRange();
    }
}
