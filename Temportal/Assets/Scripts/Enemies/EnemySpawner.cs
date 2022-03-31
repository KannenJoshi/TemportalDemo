using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int enemiesPerWave = 3;
    [SerializeField] private Vector3[] spawnLocations;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float delayBetweenWaves = 5.0f;

    private GameObject[] enemiesActive;
    
    void Start()
    {
        enemiesActive = new GameObject[enemiesPerWave];
    }

    void Update()
    {
        
    }

    private GameObject SpawnEnemy(GameObject type, Vector3 position)
    {
        return Instantiate(type, position, Quaternion.identity);
    }

    private void SpawnWave()
    {
        enemiesActive = new GameObject[enemiesPerWave];

        ArrayList possibleLocations = new ArrayList(spawnLocations);
        for (int i = 0; i < enemiesPerWave; i++)
        {
            var posIndex = Random.Range(0, possibleLocations.Count);
            var pos = (Vector3) possibleLocations[posIndex];
            possibleLocations.RemoveAt(i);
            
            var enemyIndex = Random.Range(0, enemyPrefabs.Length);
            var enemy = enemyPrefabs[enemyIndex];

            SpawnEnemy(enemy, pos);
        }
    }
}
