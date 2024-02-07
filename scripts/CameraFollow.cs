using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Reference to the player character
    public float smoothness = 5f; // Smoothing factor for camera movement
    public float coverage = 15f; // Desired coverage of the scene

    void LateUpdate()
    {
        if (target != null)
        {
            // Calculate the target position for the camera
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);

            // Smoothly move the camera towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothness * Time.deltaTime);

            // Calculate the desired orthographic size based on coverage and aspect ratio
            float targetOrthographicSize = coverage * Screen.height / Screen.width * 0.5f;

            // Smoothly adjust the orthographic size towards the target size
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetOrthographicSize, smoothness * Time.deltaTime);
        }
    }
}
