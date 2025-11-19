using UnityEngine;

public class playerAttack : StateMachineBehaviour
{
    public AudioClip attackSound;
     [Header("TrailRenderer")]
    // public TrailRenderer swordTrail;
    [HideInInspector] public TrailRenderer trail;

    
     public void SetTrail(TrailRenderer t)
    {
        trail = t;
    }
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Attack state entered!");

        if (attackSound != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(attackSound);

        }
        else
        {
            Debug.LogWarning("attackSound or AudioManager.Instance is null!");
        }
         if (trail != null)
            trail.emitting = true;
            
    }
 override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       if (trail != null)
            trail.emitting = false;

        
    }

}
