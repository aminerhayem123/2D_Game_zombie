using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public GameObject bulletPrefab; // Reference to the bullet prefab
    public Transform bulletSpawnPoint; // Position to spawn the bullet
    public float bulletForce = 10f; // Force to apply to the bullet

    private Rigidbody2D rb; // Rigidbody2D component for physics-based movement

    void Start()
    {
        // Get the Rigidbody2D component attached to the same GameObject
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Get input for movement
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Calculate movement vector
        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized * speed;

        // Move the character using Rigidbody2D
        rb.velocity = movement;

        // Rotate character to face the mouse
        RotateCharacterTowardsMouse();

        // Check for left mouse button click
        if (Input.GetMouseButtonDown(0))
        {
            // Shoot a bullet
            ShootBullet();
        }
    }

    void RotateCharacterTowardsMouse()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePosition - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust the angle to make the character face the correct direction
        angle -= 90f;

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void ShootBullet()
    {
        // Check if the bulletPrefab and bulletSpawnPoint are assigned
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            // Instantiate a bullet at the specified position and rotation
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);

            // Get the Rigidbody2D component of the instantiated bullet
            Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();

            // Apply force to the bullet in the direction of the player's rotation
            bulletRb.AddForce(bulletSpawnPoint.up * bulletForce, ForceMode2D.Impulse);

            // Destroy the bullet after 5 seconds
            Destroy(bullet, 3f);
        }
    }


}
