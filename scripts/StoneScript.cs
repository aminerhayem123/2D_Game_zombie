using UnityEngine;

public class StoneScript : MonoBehaviour
{
    public float stonedDuration = 4f;
    private bool isStoned = false;
    private float stonedTimer = 0f;

    // Add this property
    public bool IsStoned
    {
        get { return isStoned; }
    }

    void Update()
    {
        if (isStoned)
        {
            stonedTimer += Time.deltaTime;

            if (stonedTimer >= stonedDuration)
            {
                isStoned = false;
                stonedTimer = 0f;
                Debug.Log("Character is no longer stoned!");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpiderThread") && !isStoned)
        {
            isStoned = true;
            Debug.Log("Character is stoned!");

            // Additional logic to disable movement scripts or play animations could go here

            Invoke("ResetStonedState", stonedDuration);
        }
    }

    void ResetStonedState()
    {
        isStoned = false;
        stonedTimer = 0f;
        Debug.Log("Stoned state reset!");
    }
}
