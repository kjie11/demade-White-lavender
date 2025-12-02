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

    //choose weapon
        public enum WeaponType
    {
        Knife,      // 原来的近战武器
        ThrowBall   // 新增的投掷球
    }
    [Header("Weapon Settings")]
public WeaponType currentWeapon = WeaponType.Knife;
[Header("Throw Ball Settings")]
public GameObject ballPrefab;
public Transform throwPoint; 
public float throwForce = 20f;



    //exhausted bar event
    public event Action OnAttack;
    [Header("Knife trail effect setting")]
     public TrailRenderer swordTrail;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // characterController=GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        //set the knife trail effect render
        var behaviours = animator.GetBehaviours<playerAttack>();
        foreach (var b in behaviours)
        {
            b.SetTrail(swordTrail);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0) && Time.time >= nextAttackTime)
        {
            if (currentWeapon == WeaponType.Knife)
            {
                
                Attack();
            }
            else if (currentWeapon == WeaponType.ThrowBall)
            {
                
                ThrowBall();
            }
            
            nextAttackTime = Time.time + attackCooldown;
        }
           
           //有问题 先不要后翻滚
            // if(Input.GetKeyDown(backRollKey)&&!isRolling&&Time.time>=nextRollTime){
            //     // StartCoroutine(BackRoll());
            //     BackRoll();
            // }
    }


   void ThrowBall()
{
    animator.SetTrigger("Throw"); // 播放投掷动画（你已有）

    // 投掷动作延迟出球（可选）
    StartCoroutine(DelayedThrow());
}

IEnumerator DelayedThrow()
{
    yield return new WaitForSeconds(0.2f); // 与动画同步

    GameObject ball = Instantiate(ballPrefab, throwPoint.position, throwPoint.rotation);
    // Rigidbody rb = ball.GetComponent<Rigidbody>();

    // if (rb != null)
    // {
    //     rb.AddForce(transform.forward * throwForce, ForceMode.Impulse);
    // }
    ball.GetComponent<ThrowBall>().Throw(transform.forward, throwForce);
}
    void Attack(){
            animator.SetTrigger("Attack");
             OnAttack?.Invoke(); //notify exhaustedBar
            Vector3 center=transform.position;
            Collider[] hits=Physics.OverlapSphere(center,attackRange, enemyLayer);
            // Collider[] hits = Physics.OverlapSphere(center, attackRange);


        if (hits.Length == 0)
        {
            Debug.Log("hits =0");
            return;
        }
            Debug.Log("hit is not zero");
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
                nearest.GetComponent<StaticEnemyHealth>()?.TakeDamage(attackDamage);
            }
            }
            else{
                foreach(var c in hits)
            {
                if (!IsFacingTarget(c.transform, center)) continue;
                 c.GetComponent<enemyHealth>()?.takeDamage(attackDamage);
                 c.GetComponent<StaticEnemyHealth>()?.TakeDamage(attackDamage);
            }
               
            }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0.5f, 0, 0.25f);
        Gizmos.DrawSphere(transform.position, attackRange);
    }
    
    void BackRoll()
{
    Debug.Log("In the back roll");
    isRolling = true;
    nextRollTime = Time.time + backRollCooldown;

        animator.SetBool("back", true);
        animator.SetBool("FreeFall", false);
    animator.applyRootMotion = true;

    StartCoroutine(ResetBackRollAfterDelay(backRollDuration));
}

private IEnumerator ResetBackRollAfterDelay(float delay)
{
    yield return new WaitForSeconds(delay);
    animator.SetBool("back", false);
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
