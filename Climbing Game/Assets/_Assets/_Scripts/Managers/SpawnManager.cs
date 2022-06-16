using System;
using Cinemachine;
using UnityEngine;
using GamerWolf.Utils;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class SpawnManager : MonoBehaviour {
    [Header("Spawning Logs")]
    [SerializeField] private Transform variationsParent;
    [SerializeField] private CinemachineVirtualCamera followCam;
    [SerializeField] private string[] poolName;
    [SerializeField] private Transform intialSpawnPoint;
    [SerializeField] private int maxLogDestoryToSpawn = 1;

    [Header("Enemy Spawning")]
    [SerializeField] private Transform[] enemySpawnPoints;
    private LevelVariations currentVariations;
    private ObjectPoolingManager poolingManager;
    private int nextLogSpawnAmount;
    private bool canSpawn;
    
    public Action onLogDistroy;
    public static SpawnManager current;
    private void Awake(){
        current = this;
        poolingManager = ObjectPoolingManager.current;
    }

    private void Start(){
        Init();
        onLogDistroy += ()=>{
            nextLogSpawnAmount = 0;
            SpawnOtherLogs();
        };
    }
    public void Init(){
        int rand = Random.Range(0,poolName.Length);
        GameObject variations = poolingManager.SpawnFromPool(poolName[rand],intialSpawnPoint.position,intialSpawnPoint.rotation,variationsParent);
        if(variations.TryGetComponent<LevelVariations>(out LevelVariations newVaritaionsRight)){
            currentVariations = newVaritaionsRight;

        }

        for (int i = 0; i < 10; i++){
            SpawnOtherLogs();
        }
        StartCoroutine(SpawnEnemy());
    }
    public void SpawnOtherLogs(){
        int rand = Random.Range(0,poolName.Length);
        GameObject variations = poolingManager.SpawnFromPool(poolName[rand],currentVariations.GetNextObstacleSpawnPoint().position,currentVariations.GetNextObstacleSpawnPoint().rotation,variationsParent);
        
        if(variations.TryGetComponent<LevelVariations>(out LevelVariations newVaritaionsRight)){
            currentVariations = newVaritaionsRight;
        }
        
    }
    private IEnumerator SpawnEnemy(){
        yield return new WaitForSeconds(5f);
        int rand = Random.Range(1,3);
        for (int i = 0; i < rand; i++){
            int ran = Random.Range(0,enemySpawnPoints.Length);
            poolingManager.SpawnFromPool("Spider",enemySpawnPoints[ran].position,enemySpawnPoints[ran].rotation);
            yield return new WaitForSeconds(1f);
        }
        yield return StartCoroutine(SpawnEnemy());
    }
    public void InvokeSpawnNewSection(){
        nextLogSpawnAmount++;
        // currentVariationsList.Add(removeVariation);
        if(nextLogSpawnAmount >= maxLogDestoryToSpawn){
            onLogDistroy?.Invoke();
        }
    }
    
}
