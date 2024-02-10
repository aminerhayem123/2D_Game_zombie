using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightController : MonoBehaviour
{
    public GameObject lightNormal;
    public GameObject lightDanger;
    public float dangerRange = 5f;
    private GameObject currentEnemy;

    private void Update()
    {
        CheckForEnemy();

        // Check if the red light is on and there's a current enemy within danger range
        if (IsRedLightOn() && currentEnemy != null && IsEnemyWithinDangerRange())
        {
            // If the conditions are met, make the enemy follow the player
            MoveEnemyTowardsPlayer();
        }
    }

    void CheckForEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");

        if (enemies.Length > 0)
        {
            float minDistance = float.MaxValue;
            GameObject closestEnemy = null;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestEnemy = enemy;
                }
            }

            if (minDistance <= dangerRange)
            {
                ActivateLight(lightDanger);
                currentEnemy = closestEnemy;
            }
            else
            {
                ActivateLight(lightNormal);
                currentEnemy = null;
            }
        }
        else
        {
            ActivateLight(lightNormal);
            currentEnemy = null;
        }
    }

    void ActivateLight(GameObject lightObject)
    {
        if (lightObject == lightDanger)
        {
            lightDanger.SetActive(true);
            lightNormal.SetActive(false);
        }
        else
        {
            lightNormal.SetActive(true);
            lightDanger.SetActive(false);
        }
    }

    // Check if the red light is on
    public bool IsRedLightOn()
    {
        return lightDanger.activeSelf;
    }

    // Get the danger range
    public float GetDangerRange()
    {
        return dangerRange;
    }

    // Check if the current enemy is within danger range
    bool IsEnemyWithinDangerRange()
    {
        float distance = Vector3.Distance(transform.position, currentEnemy.transform.position);
        return distance <= dangerRange;
    }

    void MoveEnemyTowardsPlayer()
    {
        // Assuming the enemy script is attached to the enemyPrefab
        currentEnemy.transform.position = Vector3.MoveTowards(currentEnemy.transform.position, transform.position, Time.deltaTime);
    }
}
