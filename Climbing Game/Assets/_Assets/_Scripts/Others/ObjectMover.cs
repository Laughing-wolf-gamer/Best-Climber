using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectMover : MonoBehaviour {
    

    [SerializeField] private float moveSpeed;
    
    private void Update(){
        if(MasterController.current.isGamePlaying){
            transform.position += Vector3.down * moveSpeed * Time.deltaTime;
        }
    }

}
