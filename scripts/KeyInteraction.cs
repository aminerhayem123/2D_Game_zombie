using UnityEngine;

public class KeyInteraction : MonoBehaviour
{
    public GameObject spider; // Reference to the spider GameObject
    public int healthToAdd = 5; // Amount of health to add to the player character

    // Array to hold possible spawn points for the key
    public Transform[] spawnPoints;

    // Boolean to track whether the key has been collected
    private bool keyCollected = false;
    private float visibilityTimer = 7f; // Set an initial value for the timer

    // Reference to the CameraFollow script
    public CameraFollow cameraFollow;

    void Start()
    {
        // Move the key to a random spawn point when the game starts
        MoveToRandomSpawnPoint();
    }

    void Update()
    {
        // Check if the key has been collected
        if (keyCollected)
        {
            // Countdown the visibility timer
            visibilityTimer -= Time.deltaTime;

            // Gradually make the object invisible
            Color currentColor = GetComponent<SpriteRenderer>().color;
            currentColor.a = Mathf.Clamp01(visibilityTimer / 5f); // Adjust the division value as needed
            GetComponent<SpriteRenderer>().color = currentColor;

            // Check if the timer has reached 0
            if (visibilityTimer <= 0)
            {
                // Activate the spider
                ActivateSpider();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!keyCollected && other.CompareTag("caracter"))
        {
            // Modify the player character's health
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // Restore health instead of modifying it
                playerController.RestoreHealth(healthToAdd);
            }

            // Set keyCollected to true so it doesn't spawn again
            keyCollected = true;

            // Call the StartCameraTransition method in CameraFollow
            if (cameraFollow != null)
            {
                cameraFollow.StartCameraTransition();
            }
            else
            {
                Debug.LogError("CameraFollow script not assigned in the Inspector.");
            }
        }
    }

    // Function to move the key to a random spawn point from the array
    private void MoveToRandomSpawnPoint()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned to the KeyInteraction script.");
            return;
        }

        // Choose a random spawn point
        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Set the key's position to the chosen spawn point
        transform.position = randomSpawnPoint.position;
    }

    // Method to activate the spider object and initiate camera transition
    private void ActivateSpider()
    {
        if (spider != null)
        {
            spider.SetActive(true);
        }
        else
        {
            Debug.LogError("Spider not assigned in the Inspector.");
        }
    }
}
