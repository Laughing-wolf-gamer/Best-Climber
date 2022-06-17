using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Food : Interactable{

    [SerializeField] private float moveSpeed;
    private MasterController masterController;
    private void Start(){
        masterController = MasterController.current;
    }
    private void Update(){
        if(masterController.isGamePlaying){
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }
    public override void Interact(PlayerCollision player){
        base.Interact(player);
        Debug.Log("Collided With Food");
        DestroyNow();
    }
    

}
