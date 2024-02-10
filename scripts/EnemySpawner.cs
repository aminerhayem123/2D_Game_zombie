using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float spawnInterval = 2f;
    public int maxEnemies = 10;
    public float spawnRange = 5f;
    public float activationRange = 10f;
    private float timer;
    private Transform player;

    void Start()
    {
        // Find the player by tag (adjust as needed)
        GameObject playerObject = GameObject.FindGameObjectWithTag("caracter");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found. Make sure the player has the correct tag.");
        }
    }

    void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= activationRange)
        {
            if (CountEnemies() < maxEnemies)
            {
                timer += Time.deltaTime;

                if (timer >= spawnInterval)
                {
                    SpawnEnemy(GetRandomSpawnPosition());
                    timer = 0f;
                }
            }
        }
    }

    void SpawnEnemy(Vector3 spawnPosition)
    {
        if (enemyPrefab != null)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enemy.transform.parent = transform;
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        return transform.position + new Vector3(Random.Range(-spawnRange, spawnRange), Random.Range(-spawnRange, spawnRange), 0f);
    }

    int CountEnemies()
    {
        return transform.childCount;
    }
}
