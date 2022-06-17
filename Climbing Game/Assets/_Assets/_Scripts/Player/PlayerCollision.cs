using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PlayerCollision : MonoBehaviour {
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Vector3 currentPos;
    private Logs currentLog;
    private bool canCheck,rightJump;
    private Rigidbody rb;
    private void Start(){
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        currentPos = transform.localPosition;
    }
    private void Update(){
        if(MasterController.current.isGamePlaying){
            transform.localPosition = currentPos;
        }
        
        tr.gameObject.SetActive(false);
        if(currentLog != null){
            Vector3 dir = (currentLog.transform.position - transform.position).normalized;
            float angle = Mathf.Atan2(dir.x,dir.z) * Mathf.Rad2Deg - 90f;
            transform.localRotation = Quaternion.AngleAxis(angle,Vector3.up);
        }else{
            if(rightJump){
                transform.localRotation = Quaternion.Euler(0f,0f,0f);
            }else{
                transform.localRotation = Quaternion.Euler(0f,-180f,0f);
            }
        }

    }
    public void TurnRight(){
        canCheck = true;
        rightJump = true;
    
    }
    
    public void TurnLeft(){
        canCheck = true;
        rightJump = false;
    }
    public void CollidedWithHazards(){
        rb.velocity = Vector3.zero;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,90f,transform.eulerAngles.z);
        Invoke(nameof(InvokeJump),0.5f);
    }
    private void InvokeJump(){
        rb.useGravity = true;
        rb.AddForce(Vector3.up * 10f ,ForceMode.Impulse);
    }
    private void OnCollisionStay(Collision coli){
        
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
