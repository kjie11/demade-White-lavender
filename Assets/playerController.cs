using UnityEngine;
using System.Collections;

public class playerController : MonoBehaviour
{
    
    [Header("Animation")]
    private Animator animator;

    [Header("Back Roll defence")]
    public KeyCode backRollKey=KeyCode.LeftControl;
    public float backRollDistance=4f;
    public float backRollDuration=0.2f;
     public float backRollCooldown = 1f; 

     private bool isRolling=false;
     private float nextRollTime=0f;
    

    // [Header("Attack")]
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        // characterController=GetComponent<CharacterController>();
        animator=GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
       
            if(Input.GetMouseButtonDown(0)){
                Attack();
            }
           
            if(Input.GetKeyDown(backRollKey)&&!isRolling&&Time.time>=nextRollTime){
                // StartCoroutine(BackRoll());
                BackRoll();
            }
    }


   
    void Attack(){
            animator.SetTrigger("Attack");
    }
    //go backward and defend
    void BackRoll(){
        Debug.Log("In the back roll");
        isRolling=true;
        nextRollTime=Time.time+backRollCooldown;
        Debug.Log("player defend");
        animator.SetTrigger("BackRoll");
        //player position go backward
        Vector3 startPos=transform.position;
        Vector3 dir=-transform.forward;
        Vector3 endPos=startPos+dir*backRollDistance;

        // //ai :update the player position smoothly
        //  float elapsed = 0f;
        // while (elapsed < backRollDuration)
        // {
        //     elapsed += Time.deltaTime;
        //     float t = Mathf.Clamp01(elapsed / backRollDuration);
        //     transform.position = Vector3.Lerp(startPos, endPos, t);
        //     yield return null;
        // }

        isRolling = false;
    }
}
