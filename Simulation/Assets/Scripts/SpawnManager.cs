using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    [AddComponentMenu("ML Agents/Spawn Manager")]
    public class SpawnManager : MonoBehaviour
    {
        public GameObject[] obstaclePrefabs;

        private Vector3 originalPosition;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private int numObstacles;
        private float hRange = 1.5f;
        private float vRange = 2.5f;
        private float minSize = 0.5f;
        private float maxSize = 1.0f;
        private float tiltAngle = 45.0f;

        // Start is called before the first frame update
        void Start()
        {
            originalPosition = gameObject.transform.position;
        }

        public void SpawnObstacles()
        {
            numObstacles = Random.Range(5, 10);
            for (int i = 0; i < numObstacles; i++)
            {
                SpawnObstacle();
            }
        }

        void SpawnObstacle()
        {
            // Get random prefab spawn point
            spawnPosition = originalPosition + new Vector3(
                Random.Range(-hRange, hRange), 
                Random.Range(3.5f, 3.5f + vRange), 
                Random.Range(-hRange, hRange));

            spawnRotation = Quaternion.Euler(new Vector3(
                Random.Range(-tiltAngle, tiltAngle),
                Random.Range(-tiltAngle, tiltAngle),
                Random.Range(-tiltAngle, tiltAngle)
            ));

            // Get random prefab index
            int obstacleIndex = Random.Range(0, obstaclePrefabs.Length);   
            GameObject obstaclePrefab = Randomize(obstaclePrefabs[obstacleIndex]);
            GameObject spawnedObstacle = Instantiate(obstaclePrefab, spawnPosition, spawnRotation);    
            spawnedObstacle.transform.parent = transform;           
        }

        private GameObject Randomize(GameObject obstacle)
        {
            // Set Random prefab size
            float randomSize = Random.Range(minSize, maxSize);
            obstacle.transform.localScale = new Vector3(randomSize, randomSize, randomSize);
            return obstacle;
        }

        public void DestroyObstacles()
        {
            foreach (Transform child in transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}