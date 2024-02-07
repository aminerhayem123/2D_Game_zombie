using UnityEngine;

public class EnemySpawnerManager : MonoBehaviour
{
    public GameObject enemySpawnerPrefab; // Reference to the enemy spawner prefab
    public int numberOfSpawners = 5; // Number of spawners to instantiate

    void Start()
    {
        SpawnEnemySpawners();
    }

    void SpawnEnemySpawners()
    {
        for (int i = 0; i < numberOfSpawners; i++)
        {
            Vector3 spawnPosition = GetRandomSpawnPosition(); // You can adjust this method based on your game's requirements
            Instantiate(enemySpawnerPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Replace this logic with your own method to generate random spawn positions
        return new Vector3(Random.Range(-10f, 10f), 0f, Random.Range(-10f, 10f));
    }
}
