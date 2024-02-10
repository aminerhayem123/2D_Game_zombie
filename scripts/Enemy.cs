using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public int health = 1;
    public AudioClip attackSound;
    private AudioSource audioSource;
    private Animator animator;
    private GameObject player;
    private lightController lightController; // Adjusted the case here
    private bool hasPlayedAttackSound = false;
    public float attackRange = 0f;
    private bool isNightModeActive = false;
    private float nightModeTimer = 0f;
    public float nightModeDuration = 4f; // Reduced to 4 seconds for testing
    public float nightModeSpeedIncrease = 2f;
    private float nightAnimationTimer = 0f;
    private bool isNightAnimationTriggered = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("caracter");
        lightController = FindObjectOfType<lightController>(); // Adjusted the case here
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Start the coroutine for triggering night mode
        StartCoroutine(TriggerNightMode());
    }

    IEnumerator TriggerNightMode()
    {
        while (true)
        {
            yield return new WaitForSeconds(4f); // Wait for 4 seconds
            StartNightMode();
        }
    }

    void StartNightMode()
    {
        isNightModeActive = true;
        nightModeTimer = 0f;
        animator.SetBool("night", true);
        // Additional logic for starting night mode can be added here
    }

    void EndNightMode()
    {
        isNightModeActive = false;
        nightModeTimer = 2f;
        animator.SetBool("night", false);
        // Additional logic for ending night mode can be added here
    }

    void Update()
    {
        if (lightController != null && IsPlayerWithinDangerRange())
        {
            MoveTowardsPlayer();

            if (ShouldAttack())
            {
                Attack();
            }

            // Reset night mode timer when in danger range
            nightModeTimer = 0f;
            isNightAnimationTriggered = false;
        }
        else
        {
            hasPlayedAttackSound = false;
            // Stop playing the walk animation when not in danger range
            animator.SetBool("walk", false);
        }

        // Check and activate night mode
        if (isNightModeActive)
        {
            nightModeTimer += Time.deltaTime;

            if (nightModeTimer >= 10f && !isNightAnimationTriggered)
            {
                // Trigger night animation after 10 seconds
                animator.SetTrigger("night");
                isNightAnimationTriggered = true;
            }

            if (nightModeTimer >= nightModeDuration)
            {
                EndNightMode();
            }
        }
    }


    void MoveTowardsPlayer()
    {
        // Implement your movement logic here

        // If moving, set the walk animation
        animator.SetBool("walk", true);
    }

    bool IsPlayerWithinDangerRange()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        return distance <= attackRange;
    }

    bool ShouldAttack()
    {
        // Implement your attack logic here
        return true;
    }

    void Attack()
    {
        // If the enemy is in night mode, player loses all health
        if (isNightModeActive)
        {
            player.GetComponent<PlayerController>().LoseAllHealth();
        }
        else
        {
            // Implement your regular attack logic here

            // Play the attack animation
            animator.SetTrigger("attack");

            PlayAttackSound();
        }
    }

    void PlayAttackSound()
    {
        if (attackSound != null && !hasPlayedAttackSound)
        {
            audioSource.PlayOneShot(attackSound);
            hasPlayedAttackSound = true;
        }
    }

    // Implement the TakeDamage method
    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            OnDeath();
        }
    }

    void OnDeath()
    {
        // Trigger the die animation
        animator.SetTrigger("die");
        // Destroy the GameObject after the death animation is complete
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("caracter"))
        {
            Attack();
        }
    }
    public bool IsNightModeActive()
    {
        return isNightModeActive;
    }
}
