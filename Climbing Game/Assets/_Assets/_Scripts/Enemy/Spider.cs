using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spider : Hazards {
    
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
    

}
