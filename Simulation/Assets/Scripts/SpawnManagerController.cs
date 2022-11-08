using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Module
{
    [AddComponentMenu("ML Agents/Spawn Manager Controller")]
    public class SpawnManagerController : MonoBehaviour
    {
        public GameObject[] obstaclePrefabs;
        private Vector3 originalPosition;
        private Vector3 spawnPosition;
        private Quaternion spawnRotation;
        private int numObstacles = 15;
        private float hRange;
        private float vRange;
        private float minSize = 0.5f;
        private float maxSize = 0.75f;
        private float tiltAngle = 45.0f;

        // Start is called before the first frame update
        void Start()
        {
            originalPosition = gameObject.transform.position;
        }

        public void SpawnObstacles()
        {
            DestroyObstacles();
            for (int i = 0; i < numObstacles; i++)
            {
                SpawnObstacle();
            }
        }

        void SpawnObstacle()
        {
            float radius = Random.Range(4.5f, 5.5f);
            spawnPosition.x = Random.Range(-1f, 1f);
            spawnPosition.y = Random.Range(0.5f, 1f);
            spawnPosition.z = Random.Range(-1f, 1f);
            spawnPosition = spawnPosition.normalized * radius;

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