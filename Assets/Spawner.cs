using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;   // Enemy3 prefab
    public int spawnCount = 3;       
    public float spawnRadius = 10f;  
    void Start()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
{
   
    Vector3 randomOffset = Random.insideUnitSphere * spawnRadius;
    randomOffset.y = 0f; 

    
    Vector3 spawnPosition = transform.position + randomOffset;

    Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
}

}
