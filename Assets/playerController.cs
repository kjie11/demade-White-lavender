using UnityEngine;

public class playerController : MonoBehaviour
{
    // [Header("Movement")]
    // public float walkSpeed=1f;
    // public float runSpeed=4f;
    // public CharacterController characterController;
    // bool isMoving = false;
    // bool isRunning = false;
    // [Header("cameraRotation")]
    // public float mouseSensitivity=2f;
    // public Transform cameraTransform;
    // private float xRotation=0f;
    [Header("Animation")]
    private Animator animator;

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
        // float x = (Input.GetKey(KeyCode.D) ? 1 : 0) + (Input.GetKey(KeyCode.A) ? -1 : 0);
        // float z = (Input.GetKey(KeyCode.W) ? 1 : 0) + (Input.GetKey(KeyCode.S) ? -1 : 0); //ai
        // Vector3 move = new Vector3(x, 0f, z);
        // characterController.Move(move * walkSpeed * Time.deltaTime);

        // isMoving= move.sqrMagnitude > 0.0001f;
        // isRunning=Input.GetKey(KeyCode.LeftShift)|| Input.GetKey(KeyCode.RightShift);
        //     if (isMoving)
        // {
        //     if(isRunning){
        //         animator.SetFloat("Speed", runSpeed);
        //     }
        //     else{
        //         animator.SetFloat("Speed", walkSpeed);
        //     }
             
        // }
        // else
        // {
        //     animator.SetFloat("Speed", 0f); 
        // }
        //     HandleCameraRotation();

            // check the attack
            if(Input.GetMouseButtonDown(0)){
                Attack();
            }
        }


    // void HandleCameraRotation(){
    //     float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
    //     float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

    //     transform.Rotate(Vector3.up * mouseX);

    //     xRotation -= mouseY;
    //     xRotation = Mathf.Clamp(xRotation, -80f, 80f);
    //     cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    // }
    void Attack(){
            animator.SetTrigger("Attack");
    }
}
