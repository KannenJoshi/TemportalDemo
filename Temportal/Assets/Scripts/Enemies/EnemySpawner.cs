using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private int enemiesPerWave = 3;
    [SerializeField] private Transform[] spawnTransforms;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private float delayBetweenWaves = 5.0f;

    private List<Vector3> spawnLocations;
    private GameObject[] enemiesActive;
    private bool delayWaves;
    private float delayStart;
    
    void Start()
    {
        spawnLocations = new List<Vector3>();
        enemiesActive = new GameObject[enemiesPerWave];

        foreach (var loc in spawnTransforms)
        {
            spawnLocations.Add(loc.position);
        }
    }

    void Update()
    {
        enemiesActive = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemiesActive.Length == 0 && !delayWaves)
        {
            delayStart = Time.time;
            delayWaves = true;
        }
        else if (delayWaves && Time.time - delayStart > delayBetweenWaves)
        {
            SpawnWave();
            delayWaves = false;
        }
    }

    private GameObject SpawnEnemy(GameObject type, Vector3 position)
    {
        return Instantiate(type, position, Quaternion.identity);
    }

    private void SpawnWave()
    {
        //enemiesActive = new GameObject[enemiesPerWave];

        // Copy List
        var possibleLocations = new List<Vector3>(spawnLocations);
        for (int i = 0; i < enemiesPerWave; i++)
        {
            var posIndex = Random.Range(0, possibleLocations.Count);
            var pos = (Vector3) possibleLocations[posIndex];
            possibleLocations.RemoveAt(i);
            
            var enemyIndex = Random.Range(0, enemyPrefabs.Length);
            var enemyFab = enemyPrefabs[enemyIndex];

            var enemyGO = SpawnEnemy(enemyFab, pos);
            var enemy = enemyGO.GetComponent<NPC>();

            var points = new List<Vector3>();

            for (int j = 0; j < spawnLocations.Count; j++)
            {
                var index = (j + posIndex) % spawnLocations.Count;
                points.Add(spawnLocations[index]);
            }
            enemy.SetPatrolPoints(points);
        }
    }
}
