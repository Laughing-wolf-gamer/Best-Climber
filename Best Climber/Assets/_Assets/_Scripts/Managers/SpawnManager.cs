using System;
using Cinemachine;
using UnityEngine;
using GamerWolf.Utils;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
public class SpawnManager : MonoBehaviour {
    [Header("Spawning Plane")]
    [SerializeField] private CinemachineVirtualCamera followCam;
    [SerializeField] private Transform planeSpawnPoint;



    [SerializeField] private string[] poolName;
    [SerializeField] private Transform initialRingSpawnPos;
    [SerializeField] private Logs currentVariations;
    private ObjectPoolingManager poolingManager;
    public int nextRingSpawnAmount;

    private Vector3 spawnPoint;
    private bool canSpawn;
    
    public Action onRingDistroy;
    public static SpawnManager current;
    private void Awake(){
        current = this;
        poolingManager = ObjectPoolingManager.current;
    }

    private void Start(){
        Init();
        onRingDistroy += ()=>{
            nextRingSpawnAmount = 0;
            SpawnOtherRings();
        };
    }
    public void Init(){
        int rand = Random.Range(0,poolName.Length);

        GameObject ring = poolingManager.SpawnFromPool(poolName[rand],initialRingSpawnPos.position,initialRingSpawnPos.rotation);
        if(ring.TryGetComponent<Logs>(out Logs newVaritaions)){
            currentVariations = newVaritaions;
        }
        for (int i = 0; i < 10; i++){
            SpawnOtherRings();
        }
    }
    public void SpawnOtherRings(){
        
        spawnPoint = currentVariations.GetNextObstacleSpawnPoint().position + new Vector3(Random.Range(-30f,30f),Random.Range(-20f,20f),Random.Range(-180f,180f));
        Quaternion newRot = currentVariations.GetNextObstacleSpawnPoint().rotation * Quaternion.Euler(Vector3.up * Random.Range(-90f,90f));
        int rand = Random.Range(0,10);
        if(rand > 3){
            poolingManager.SpawnFromPool("Fuel",spawnPoint + Vector3.forward * 20,newRot);
        }
        
        int ringRand = Random.Range(0,poolName.Length);
        GameObject ring = poolingManager.SpawnFromPool(poolName[ringRand],spawnPoint,newRot);
        if(ring.TryGetComponent<Logs>(out Logs newVaritaions)){
            currentVariations = newVaritaions;
        }
    }
    public void InvokeSpawnNewSection(){
        nextRingSpawnAmount++;
        if(nextRingSpawnAmount > 5){
            onRingDistroy?.Invoke();
        }
    }
    
}
