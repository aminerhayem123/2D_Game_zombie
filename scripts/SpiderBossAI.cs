using System.Collections;
using UnityEngine;

public class SpiderBossAI : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 3f;
    public float rotationSpeed = 200f;
    public float shootInterval = 10f;
    public GameObject bulletPrefab;
    public Transform shootPosition;
    public float deviationAngle = 10f;
    public float bulletForce = 5f;

    public AudioClip walkSound;
    public AudioClip attackSound;
    public AudioClip jumpSound;
    private AudioSource audioSource;

    public int maxHealth = 20;
    private int currentHealth;

    public GameObject healthBarCube;
    private float originalHealthBarScaleX;

    public float jumpInterval = 10f;
    private float timeSinceLastJump;

    private GameObject[] threads; // Keep track of created threads

    void Start()
    {
        currentHealth = maxHealth;

        timeSinceLastJump = jumpInterval;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.5f;

        threads = new GameObject[0]; // Initialize threads array

        // Start shooting bullets every 4 seconds
        InvokeRepeating("ShootBullets", 0f, 6f);

        // Store the original scale of the health bar
        originalHealthBarScaleX = healthBarCube.transform.localScale.x;
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 directionToTarget = target.position - transform.position;
            directionToTarget.Normalize();

            float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg - 90f;

            float angleDelta = rotationSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, angle), angleDelta);

            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            if (!audioSource.isPlaying && walkSound != null)
            {
                audioSource.clip = walkSound;
                audioSource.Play();
            }

            timeSinceLastJump += Time.deltaTime;
            if (timeSinceLastJump >= jumpInterval)
            {
                Jump();
                timeSinceLastJump = 0f;
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    void Die()
    {
        Debug.Log("Spider has been defeated!");

        // Destroy all existing threads
        DestroyThreads();

        // Destroy the spider
        Destroy(gameObject);
    }

    void DestroyThreads()
    {
        // Destroy all existing threads
        foreach (GameObject thread in threads)
        {
            Destroy(thread);
        }

        // Clear the threads array
        threads = new GameObject[0];
    }

    void Shoot()
    {
        // Check if the bullet prefab and shoot position are assigned.
        if (bulletPrefab != null && shootPosition != null)
        {
            // Apply a random deviation to the direction.
            float randomDeviation = Random.Range(-deviationAngle, deviationAngle);
            Quaternion deviationQuaternion = Quaternion.AngleAxis(randomDeviation, Vector3.forward);
            Vector3 finalDirection = deviationQuaternion * transform.up;

            // Instantiate the bullet at the shoot position's position and rotation.
            GameObject bullet = Instantiate(bulletPrefab, shootPosition.position, Quaternion.LookRotation(Vector3.forward, finalDirection));

            // Apply force to the bullet.
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.AddForce(finalDirection * bulletForce, ForceMode2D.Impulse);
            }
            else
            {
                Debug.LogError("Rigidbody2D component not found on the bullet prefab.");
            }

            // Play the attack sound when shooting.
            if (attackSound != null)
            {
                audioSource.clip = attackSound;
                audioSource.PlayOneShot(attackSound);
            }

            // Add the thread to the threads array
            threads = AddThreadToArray(threads, bullet);
        }
        else
        {
            Debug.LogError("Bullet prefab or shoot position not assigned in the Inspector.");
        }
    }

    void Jump()
    {
        float jumpHeight = 5f;
        float jumpDuration = 1f;

        if (jumpSound != null)
        {
            audioSource.clip = jumpSound;
            audioSource.PlayOneShot(jumpSound);
        }

        StartCoroutine(JumpAnimation(jumpHeight, jumpDuration));
    }

    IEnumerator JumpAnimation(float jumpHeight, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = target.position + Vector3.up * jumpHeight;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the spider is exactly at the target position
        transform.position = targetPosition;
    }

    void ShootBullets()
    {
        for (int i = 0; i < 5; i++)
        {
            Shoot();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("bullet"))
        {
            TakeDamage(1);
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("caracter"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.TakeDamage(2); // Adjust the damage value as needed
            }
        }
    }

    void TakeDamage(int damage)
    {
        Debug.Log($"Taking damage: {damage}");
        currentHealth -= damage;
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    void UpdateHealthBar()
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        float newScaleX = Mathf.Clamp(healthPercentage, 0f, 1f) * originalHealthBarScaleX;
        healthBarCube.transform.localScale = new Vector3(newScaleX, healthBarCube.transform.localScale.y, healthBarCube.transform.localScale.z);
    }

    // Helper method to add a thread to the threads array
    GameObject[] AddThreadToArray(GameObject[] array, GameObject thread)
    {
        GameObject[] newArray = new GameObject[array.Length + 1];
        for (int i = 0; i < array.Length; i++)
        {
            newArray[i] = array[i];
        }
        newArray[array.Length] = thread;
        return newArray;
    }
}