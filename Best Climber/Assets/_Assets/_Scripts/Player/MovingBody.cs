using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovingBody : MonoBehaviour {
    [SerializeField] private float upMoveSpeed;


    private void Update(){
        transform.position += Vector3.up * upMoveSpeed * Time.deltaTime;
    }
}
