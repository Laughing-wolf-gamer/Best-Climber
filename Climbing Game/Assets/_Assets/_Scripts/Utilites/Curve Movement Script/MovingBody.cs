using UnityEngine;
using GamerWolf.Utility;
public class MovingBody : MonoBehaviour {
    [SerializeField] private CurveSetUp curveSetUp;
    [SerializeField] private SwipeDetection swipeDetection;
    [SerializeField] private PlayerCollision body;
    private void Awake(){
        curveSetUp.SetUp(this.transform);
    }
    private void Start(){
        swipeDetection.OnSwipe += (float x,float y) =>{
            Debug.Log("Swipe Detected");
            if(x > 0){
                curveSetUp.TryMoveRight();
                if(curveSetUp.IsJumping()){
                    body.TurnRight();
                }
            }else if(x < 0){
                curveSetUp.TryMoveLeft();
                if(curveSetUp.IsJumping()){
                    body.TurnLeft();
                }
            }
        };
    }

    private void Update(){
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            curveSetUp.TryMoveRight();
            
        }else if(Input.GetKeyDown(KeyCode.LeftArrow)){
            
            curveSetUp.TryMoveLeft();
        }
        
    }
    
}

