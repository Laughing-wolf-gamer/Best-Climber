using UnityEngine;
using GamerWolf.Utility;
using System.Collections;
using System.Collections.Generic;
public class PlayerCollision : MonoBehaviour {
    private Vector3 currentPos;
    private Logs currentLog;
    private void Start(){
        currentPos = transform.localPosition;
    }
    private void Update(){
        transform.localPosition = currentPos;
        if(currentLog != null){
            Vector3 dir = (currentLog.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.x,dir.z) * Mathf.Rad2Deg - 90f;
            transform.localRotation = Quaternion.AngleAxis(angle,Vector3.up);
        }

    }
    public void TurnRight(){
        currentLog = null;
        transform.localRotation = Quaternion.Euler(0f,0f,0f);
    }
    public void TurnLeft(){
        currentLog = null;
        transform.localRotation = Quaternion.Euler(0f,-180f,0f);
    }
    private void OnCollisionEnter(Collision coli){
        if(coli.transform.TryGetComponent<Logs>(out Logs newLog)){
            currentLog = newLog;
        }
    }
    private void OnCollisionExit(Collision coli){
        if(coli.transform.TryGetComponent<Logs>(out Logs newLog)){
            currentLog = null;
        }
    }
}
