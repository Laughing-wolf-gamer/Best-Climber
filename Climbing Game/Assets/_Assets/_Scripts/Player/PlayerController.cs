using UnityEngine;
using System.Collections;
using Baracuda.Monitoring;
using Baracuda.Monitoring.API;
using System.Collections.Generic;


public class PlayerController : MonoBehaviour {
    


    [SerializeField] private float turningSpeed;

    [SerializeField] private float jumpSpeed;
    [SerializeField] private float desiredRotation;
    [SerializeField] private Transform leftLog,CenterLog,RightLog;
    private float currentTurnSpeed;
    private float currentRot;
    private void Update(){
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            desiredRotation = -90;
        }else if(Input.GetKeyDown(KeyCode.LeftArrow)){
            desiredRotation = 90f;
        }
    }
    private void LateUpdate(){
        if(currentRot != desiredRotation){
            currentRot = Mathf.SmoothDamp(currentRot,desiredRotation,ref currentTurnSpeed,turningSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x,currentRot,transform.eulerAngles.z);
        }

    }
}


