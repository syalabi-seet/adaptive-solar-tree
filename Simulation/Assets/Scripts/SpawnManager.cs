using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public bool spawnMode = false;
    public GameObject[] obstaclePrefab;

    private Vector3 origin;
    private Vector3 spawnPos;
    private int numObstacles;
    private float range = 2.0f;
    private float minSize = 0.1f;
    private float maxSize = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Get origin
        origin = gameObject.transform.position;

        // Spawn random obstacles
        if (spawnMode)
        {
            numObstacles = Random.Range(5, 10);
            for (int i = 0; i < numObstacles; i++)
            {
                SpawnObstacle();
            }
        }
    }

    void SpawnObstacle()
    {
        // Get random prefab spawn point
        spawnPos = origin + new Vector3(
            Random.Range(-range, range), 
            Random.Range(1, range), 
            Random.Range(-range, range));

        // Get random prefab index
        int obstacleIndex = Random.Range(0, obstaclePrefab.Length);   
        GameObject obstacle = obstaclePrefab[obstacleIndex];
        obstacle = RandomPrefab(obstacle);
        Instantiate(obstacle, spawnPos, obstacle.transform.rotation);       
    }

    private GameObject RandomPrefab(GameObject obstacle)
    {
        // Set Random prefab size
        float randomSize = Random.Range(minSize, maxSize);
        obstacle.transform.localScale = new Vector3(randomSize, randomSize, randomSize);
        return obstacle;
    }
}
