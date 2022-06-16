using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Hazards : MonoBehaviour {
    

    private void OnTriggerEnter(Collider coli){
        if(coli.TryGetComponent<PlayerCollision>(out PlayerCollision player)){
            
            MasterController.current.SetGameOver();
        }
    }

}
