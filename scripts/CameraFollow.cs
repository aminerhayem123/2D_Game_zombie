using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Transform targetObject;  // Add the target object for camera focus
    public float smoothness = 5f;
    public float coverage = 15f;

    private bool isTransitioning = false;
    private float transitionTimer = 6f;  // Adjust the time as needed

    public Canvas legacyCanvas;
    public Canvas otherCanvas;  // Reference to the other canvas to deactivate

    void LateUpdate()
    {
        if (isTransitioning)
        {
            HandleTransition();
        }
        else
        {
            HandleCameraPosition();
        }

        CheckCameraFocus();
    }

    void HandleCameraPosition()
    {
        Vector3 targetPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothness * Time.deltaTime);

        float targetOrthographicSize = coverage * Screen.height / Screen.width * 0.5f;
        Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetOrthographicSize, smoothness * Time.deltaTime);
    }

    void HandleTransition()
    {
        // Move the camera towards the target object during transition
        Vector3 targetPosition = new Vector3(targetObject.position.x, targetObject.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothness * Time.deltaTime);

        float targetOrthographicSize = coverage * Screen.height / Screen.width * 0.5f;

        // Check if the camera is returning to the player
        if (IsCameraFocusedOnPlayer())
        {
            // Gradually increase the orthographic size
            targetOrthographicSize = Mathf.Lerp(Camera.main.orthographicSize, targetOrthographicSize, smoothness * Time.deltaTime);
        }

        Camera.main.orthographicSize = targetOrthographicSize;

        // Countdown the transition timer
        transitionTimer -= Time.deltaTime;

        if (transitionTimer <= 0f)
        {
            // Reset the transition state and timer
            EndTransition();
        }
    }

    void CheckCameraFocus()
    {
        // Check if the camera is focused on the target object
        if (player != null && targetObject != null && IsCameraFocusedOnTarget())
        {
            // Show Legacy text
            legacyCanvas.gameObject.SetActive(true);

            // Deactivate otherCanvas
            if (otherCanvas != null)
            {
                otherCanvas.gameObject.SetActive(false);
            }
        }
        else
        {
            // Hide Legacy text
            legacyCanvas.gameObject.SetActive(false);

            // Activate otherCanvas
            if (otherCanvas != null)
            {
                otherCanvas.gameObject.SetActive(true);
            }
        }
    }

    bool IsCameraFocusedOnTarget()
    {
        // Check if the camera is focused on the target within a certain threshold
        float positionThreshold = 0.1f;
        float sizeThreshold = 0.1f;

        Vector3 cameraPosition = new Vector3(transform.position.x, transform.position.y, 0f);
        Vector3 targetPosition = new Vector3(targetObject.position.x, targetObject.position.y, 0f);

        return Vector3.Distance(cameraPosition, targetPosition) < positionThreshold
            && Mathf.Abs(Camera.main.orthographicSize - (coverage * Screen.height / Screen.width * 0.5f)) < sizeThreshold;
    }

    bool IsCameraFocusedOnPlayer()
    {
        // Check if the camera is focused on the player within a certain threshold
        float positionThreshold = 0.1f;
        float sizeThreshold = 0.1f;

        Vector3 cameraPosition = new Vector3(transform.position.x, transform.position.y, 0f);
        Vector3 playerPosition = new Vector3(player.position.x, player.position.y, 0f);

        return Vector3.Distance(cameraPosition, playerPosition) < positionThreshold
            && Mathf.Abs(Camera.main.orthographicSize - (coverage * Screen.height / Screen.width * 0.5f)) < sizeThreshold;
    }

    // Call this method when you want to initiate camera transition
    public void StartCameraTransition()
    {
        isTransitioning = true;
    }

    // Call this method to end the camera transition
    void EndTransition()
    {
        isTransitioning = false;
        transitionTimer = 4f;  // Reset the timer for the next transition
    }
}
