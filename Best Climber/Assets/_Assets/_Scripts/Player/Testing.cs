using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Testing : MonoBehaviour {
    
    [SerializeField] private float movespeed;
    [SerializeField] private Transform ancherStart,handle1,ancherEnd,handle2,currentPos;

    
    private void Update(){
        if(Vector3.Distance(transform.position,ancherEnd.position) >= 0.1f){
            movespeed += Time.deltaTime;
            handle2.transform.position = Vector3.Lerp(ancherStart.position,handle1.position,movespeed);
            currentPos.position = Vector3.Lerp(handle1.position,ancherEnd.position,movespeed);
            transform.position = Vector3.Lerp(handle2.position,currentPos.position,movespeed);
        }else{
            movespeed = 0f;
        }
    }

}
