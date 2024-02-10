using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public float bulletForce = 10f;
    public Text ammoText;
    public GameObject stonedPrefab;  // Drag your prefab into this field in the Unity Editor
    private GameObject stonedObject;


    // Health-related variables
    public int maxHealth = 5;
    private int currentHealth;

    // Ammo-related variables
    public int maxAmmo = 20;
    public int currentAmmo;

    private Rigidbody2D rb;
    private AudioSource audioSource;
    public AudioClip shootingSound;
    public AudioClip outOfAmmoSound;
    public AudioClip pickupAmmoSound;

    // UI Image components for hearts
    public Image heart1;
    public Image heart2;
    public Image heart3;
    public Image heart4;
    public Image heart5;

    public Canvas gameOverCanvas;
    public Button restartButton; // Connect this in the Unity Editor

    public Canvas playGameCanvas;
    public Button startButton; // Connect this in the Unity Editor

    private bool isGameOver = false;
    private bool gameStarted = false;
    private bool isStoned = false;
    private float stonedDuration = 3f;
    private float stonedTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentAmmo = maxAmmo;
        currentHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Disable the game over canvas initially
        if (gameOverCanvas != null)
        {
            gameOverCanvas.gameObject.SetActive(false);
        }

        if (playGameCanvas != null)
        {
            playGameCanvas.gameObject.SetActive(true);
        }

        if (startButton != null)
        {
            startButton.onClick.AddListener(StartGame);
        }
    }
    void Update()
    {
        if (gameStarted && !isGameOver)
        {
            UpdateAmmoText();

            if (!isStoned)
            {
                HandleMovementInput();
                RotateCharacterTowardsMouse();

                if (Input.GetMouseButtonDown(0))
                {
                    HandleShooting();
                }

                if (Input.GetKeyDown(KeyCode.E))
                {
                    HandleAmmoPickup();
                }

                CheckForCharacterDeath();
                UpdateHeartsUI();
            }
            else
            {
                // Player is stoned, prevent movement
                rb.velocity = Vector2.zero;
                stonedTimer += Time.deltaTime;

                if (stonedTimer >= stonedDuration)
                {
                    isStoned = false;
                    stonedTimer = 0f;
                    Debug.Log("Character is no longer stoned!");

                    // Destroy the stonedObject if it exists
                    if (stonedObject != null)
                    {
                        Destroy(stonedObject);
                    }
                }
                else
                {
                    // If stonedObject doesn't exist, instantiate it
                    if (stonedObject == null && stonedPrefab != null)
                    {
                        stonedObject = Instantiate(stonedPrefab, transform.position, Quaternion.identity);
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            HandleAmmoPickup();
        }
    }

    void HandleMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized * speed;
        rb.velocity = movement;
    }

    void HandleShooting()
    {
        if (currentAmmo > 0)
        {
            ShootBullet();
            PlayShootingSound(0.5f);
        }
        else
        {
            PlayOutOfAmmoSound();
        }
    }

    void HandleAmmoPickup()
    {
        // Check if the player has 0 ammo
        if (currentAmmo == 0)
        {
            Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, 1.5f);

            foreach (var collider in nearbyColliders)
            {
                // Check if the collider has the "AmmoBox" tag
                if (collider.CompareTag("AmmoBox"))
                {
                    // Player is close enough to the ammo box, pick up ammo
                    currentAmmo = maxAmmo;
                    Debug.Log("Picked up ammo box. Current ammo: " + currentAmmo);
                    PlayPickupAmmoSound();

                    // Disable the ammo box or perform any other necessary actions
                    Destroy(collider.gameObject); // Destroy the ammo box
                    return; // Exit the method to avoid picking up multiple ammo boxes simultaneously
                }
            }

            // If no nearby ammo box was found, print a message (optional)  
            Debug.Log("No nearby ammo box or player already has ammo.");
        }
        else
        {
            // Player already has ammo, print a message (optional)
            Debug.Log("Player already has ammo. No need to pick up.");
        }
    }

    void RotateCharacterTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle -= 90f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void ShootBullet()
    {
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
            bulletRb.AddForce(bulletSpawnPoint.up * bulletForce, ForceMode2D.Impulse);
            Destroy(bullet, 3f);

            currentAmmo--;
            Debug.Log("Shot! Ammo left: " + currentAmmo);
        }
    }

    void PlayShootingSound(float volume)
    {
        if (shootingSound != null)
        {
            audioSource.volume = volume;
            audioSource.PlayOneShot(shootingSound);
        }
    }

    void PlayOutOfAmmoSound()
    {
        if (outOfAmmoSound != null)
        {
            audioSource.PlayOneShot(outOfAmmoSound);
        }
    }

    void PlayPickupAmmoSound()
    {
        if (pickupAmmoSound != null)
        {
            audioSource.PlayOneShot(pickupAmmoSound);
        }
    }

    void CheckForCharacterDeath()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Character has died.");

        // Get the Animator component from the GameObject
        Animator animator = GetComponent<Animator>();

        // Trigger death animation
        if (animator != null)
        {
            animator.SetBool("Die", true);  // Assuming "IsDead" is the parameter name in your Animator
        }

        // Set the game over flag to true
        isGameOver = true;

        // Enable the game over canvas
        if (gameOverCanvas != null)
        {
            gameOverCanvas.gameObject.SetActive(true);
        }

        // Attach the RestartGame method to the restart button
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    void UpdateHeartsUI()
    {
        heart1.enabled = currentHealth >= 1;
        heart2.enabled = currentHealth >= 2;
        heart3.enabled = currentHealth >= 3;
        heart4.enabled = currentHealth >= 4;
        heart5.enabled = currentHealth >= 5;
        // Check if all hearts are gone
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player took damage. Current health: " + currentHealth);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("hp+1"))
        {
            // Do something when the player collides with the health pickup
            Debug.Log("Player picked up health!");

            if (currentHealth < maxHealth - 1)  // Check if health is less than max - 1 to prevent overhealing
            {
                currentHealth++;
                Debug.Log("Player gained health. Current health: " + currentHealth);
                Destroy(other.gameObject);  // Destroy the health pickup object
            }
            else
            {
                Debug.Log("Player health is already full. No need to pick up.");
            }
        }
        else if (other.CompareTag("enemy"))
        {
            // If the player collides with an enemy, check if the enemy is in night mode
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && enemy.IsNightModeActive())
            {
                LoseAllHealth();
            }
            else
            {
                // Regular enemy collision logic (e.g., take damage)
                TakeDamage(1);
            }
        }
        else if (other.CompareTag("SpiderThread"))
        {
            // Player touched spider thread, get stoned
            isStoned = true;
            stonedTimer = 0f;
            Destroy(other.gameObject);
            Debug.Log("Player is stoned!");

            // Additional logic to disable movement scripts or play animations could go here
        }
    }

    public void RestartGame()
    {
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void StartGame()
    {
        if (playGameCanvas != null)
        {
            playGameCanvas.gameObject.SetActive(false);
        }

        gameStarted = true;
    }

    void UpdateAmmoText()
    {
        // Check if ammoText is assigned
        if (ammoText != null)
        {
            // Update the text to display the current ammo count
            ammoText.text = "" + currentAmmo;
        }
    }
    public void RestoreHealth(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Player restored health. Current health: " + currentHealth);
    }

    public void LoseAllHealth()
    {
        currentHealth = 0;
        Debug.Log("Player lost all health!");
    }
}
