using UnityEngine;
using System.Collections;
using System;

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

    [Header("Player Attack")]
    public float attackDamage = 25f;          
    public float attackRange = 1.6f;           
    // public float attackForwardOffset = 0.8f;   
    // [Range(0, 180)] public float attackAngle = 120f; 
    public float hitDelay = 0.2f;              
    public LayerMask enemyLayer;              // select enemy layer in inspector
    public bool hitOnlyNearest = true;
    [Header("attack Cooldown")]
    public float attackCooldown = 1.0f;
    private float nextAttackTime = 0f;  
    //check if player face to enemy
     [Header("Facing Constraint")]
    [Range(0f, 180f)] public float attackAngle = 120f; 
    public bool requireInFront = true;                 
    

    //exhausted bar event
    public event Action OnAttack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        // characterController=GetComponent<CharacterController>();
        animator=GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
       
            if(Input.GetMouseButtonDown(0)&& Time.time >= nextAttackTime){
                Attack();
                nextAttackTime = Time.time + attackCooldown;
            }
           
            if(Input.GetKeyDown(backRollKey)&&!isRolling&&Time.time>=nextRollTime){
                // StartCoroutine(BackRoll());
                BackRoll();
            }
    }


   
    void Attack(){
            animator.SetTrigger("Attack");
             OnAttack?.Invoke(); //notify exhaustedBar
            Vector3 center=transform.position;
            Collider[] hits=Physics.OverlapSphere(center,attackRange, enemyLayer);

        if (hits.Length == 0)
        {
            Debug.Log("hits =0");
            return;
        }
            
            if(hitOnlyNearest){
                Collider nearest=null;
                float bestSqr=float.MaxValue;
            foreach (var c in hits)
            {
                if (!IsFacingTarget(c.transform, center)) continue;
                float sqr = (c.transform.position - center).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    nearest = c;
                }
            }
            if (nearest != null)
            {
                nearest.GetComponent<enemyHealth>()?.takeDamage(attackDamage);
            }
            }
            else{
                foreach(var c in hits)
            {
                if (!IsFacingTarget(c.transform, center)) continue;
                 c.GetComponent<enemyHealth>()?.takeDamage(attackDamage);
            }
               
            }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.25f);
        Gizmos.DrawSphere(transform.position, attackRange);
    }
    //go backward and defend
    void BackRoll()
    {
        Debug.Log("In the back roll");
        isRolling = true;
        nextRollTime = Time.time + backRollCooldown;
        Debug.Log("player defend");
        animator.SetTrigger("BackRoll");
        //player position go backward
        Vector3 startPos = transform.position;
        Vector3 dir = -transform.forward;
        Vector3 endPos = startPos + dir * backRollDistance;

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
    bool IsFacingTarget(Transform target, Vector3 center)
    {
        Vector3 toTarget = target.position - center;
        toTarget.y = 0f;

        if (toTarget.sqrMagnitude < 0.0001f) return true; // 几乎同一点

        // 只打前方（dot > 0）
        if (requireInFront && Vector3.Dot(transform.forward, toTarget.normalized) <= 0f)
            return false;

        // 角度限制（使用半角比较）
        float angle = Vector3.Angle(transform.forward, toTarget);
        return angle <= attackAngle * 0.5f;
    }
}
