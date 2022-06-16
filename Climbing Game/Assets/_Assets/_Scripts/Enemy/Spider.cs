using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spider : Hazards {
    
    [SerializeField] private float moveSpeed;


    private void Update(){
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
    

}
