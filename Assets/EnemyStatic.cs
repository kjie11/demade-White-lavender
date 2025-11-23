using System;
using UnityEngine;

public class EnemyStatic : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject playerObj;
    [Header("shooting Attack Settings")]
    public GameObject projectilePrefab;   
    public Transform firePoint;           
    public float projectileSpeed = 12f;
    [Header("Attack")]
    public float attackDistance=2f;
    public float attackCooldown = 3f; 
    private float nextAttackTime = 0f;
    public float attackDamageCount = 30f;
    [Header("Audio")]
    public AudioClip alertSound;          
    public float alertSoundCooldown = 5f; 
    private float lastAlertTime = -999f;
    private bool playerInRange = false;   
    private AudioSource audioSource;      
    [Header("Patrol Settings")]
    public float patrolRadius=5f;
    private Vector3 patrolCenter;
    private Vector3 patrolPoint; // ramdom get the point, make the enemy move around in the patrol area
     public float patrolWaitTime = 1f; // patrol and wait a time to change patrol point
    private float waitTimer = 0f;
    public GameObject anchor;

    [Header("tem_enemy shooting")]
    public Transform body;         // 左右旋转
public Transform cannonPivot;  // 上下旋转
// public Transform firePoint;    // 炮口
public string playerTag = "Player"; //玩家对于static enemy的更大的collider,no used

    
    
    void Start()
    {
        patrolCenter = transform.position;
         patrolPoint = patrolCenter;
        anchor.SetActive(false);
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
         bool isInRangeNow = distanceToPlayer < patrolRadius;
        if (isInRangeNow && !playerInRange)
        {
            OnPlayerEnterRange();
        }
        if (isInRangeNow)
    {
        anchor.SetActive(true);
    }
    else
    {
        // 玩家离开范围 → 隐藏 anchor
        anchor.SetActive(false);
    }
        playerInRange = isInRangeNow;
        if (distanceToPlayer < attackDistance && Time.time >= nextAttackTime)
        {
            attack();
            nextAttackTime = Time.time + attackCooldown;
        }
        if (playerInRange)
    {
        RotateBodyHorizontal();
        RotateCannonVertical();
    }
    }
    //  void OnCollisionEnter(Collision collision)
    // {
    //     // 如果撞到的是玩家
    //     if (collision.collider.CompareTag(playerTag))
    //     {
    //         // 获取玩家生命组件
    //         playerHealth ph = collision.collider.GetComponent<playerHealth>();
    //         if (ph != null)
    //         {
    //             ph.TakeDmage(attackDamageCount);
    //         }
    //     }

    //     // 销毁炮弹
    //     Destroy(gameObject);
    // }
    private void OnPlayerEnterRange()
    {
        if (alertSound != null && Time.time - lastAlertTime > alertSoundCooldown)
        {
            audioSource.PlayOneShot(alertSound);
            lastAlertTime = Time.time;
        }
          anchor.SetActive(true);
        Debug.Log("Player entered patrol range!");
    }

//     void attack()
//     {
//         Debug.Log("in static attack");
//         Vector3 look = playerTransform.position - transform.position;
//         // look.y = 0f;
//         if (look.sqrMagnitude > 0.001f)
//         {
//             transform.rotation = Quaternion.LookRotation(look);
//         }
//         // playerHealth ph = playerTransform.GetComponent<playerHealth>();
//         // if (ph != null)
//         // {
//         //     ph.TakeDmage(attackDamageCount); //todo : random damage account
//         // }
//         ShootProjectile();

//     }
//   private void ShootProjectile()
// {
//     if (projectilePrefab == null || firePoint == null)
//         return;

//     GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

//     Rigidbody rb = proj.GetComponent<Rigidbody>();

//     if (rb != null)
//         {
//         // // 玩家脚底位置
//         // Vector3 footPoint = playerObj.transform.position;
//         // // footPoint.y -= 1.3f; // 根据你的模型调整 (通常 -1 ~ -1.3)

//         //     // 关键！！！不能水平化方向！！
//         //     Vector3 dir = (footPoint - firePoint.position).normalized;
//         //   proj.transform.rotation = Quaternion.LookRotation(dir);

//         // rb.AddForce(dir * projectileSpeed, ForceMode.Impulse);
//        Vector3 footPoint = playerObj.transform.position;
// footPoint.y -= 1.3f;

// Vector3 dir = (footPoint - firePoint.position);

// // ⭐⭐⭐ 强制让方向更往下（关键！）
// dir.y = dir.y * 2f;   // 往下 2 倍斜率，你可调 1.5、2、3

// dir = dir.normalized;

// proj.transform.rotation = Quaternion.LookRotation(dir);

// rb.AddForce(dir * projectileSpeed, ForceMode.Impulse);

//     }
// }
void RotateBodyHorizontal()
{
    Vector3 dir = playerTransform.position - body.position;
    dir.y = 0f;
    if (dir.sqrMagnitude > 0.01f)
        body.rotation = Quaternion.LookRotation(dir);
}

void RotateCannonVertical()
{
    Vector3 Target = playerTransform.position;
    Target.y += 1f;
    cannonPivot.LookAt(Target);
}

void ShootProjectile()
{
    if (projectilePrefab == null || firePoint == null)
        return;

    GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    Projectile p = proj.GetComponent<Projectile>();
    p.player = playerObj;  // <-- 动态注入，永远不会丢失！

    Rigidbody rb = proj.GetComponent<Rigidbody>();
    if (rb != null)
        rb.AddForce(firePoint.forward * projectileSpeed, ForceMode.Impulse);
}

void attack()
{
    RotateBodyHorizontal();
    RotateCannonVertical();
    ShootProjectile();
}

}
