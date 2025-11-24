using System;
using UnityEngine;

public class EnemyStatic : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject playerObj;
    [Header("shooting Attack Settings")]
    public GameObject projectilePrefab;   
    public Transform firePoint;   
    public Transform firePoint2; 
    public Transform firePoint3;      

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
[Header("Projectile Material Random")]
public Material materialA;
public Material materialB;

    
    
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
    GameObject proj2 = Instantiate(projectilePrefab, firePoint2.position, firePoint2.rotation);
    GameObject proj3 = Instantiate(projectilePrefab, firePoint3.position, firePoint3.rotation);
    ApplyRandomMaterial(proj);
    ApplyRandomMaterial(proj2);
    ApplyRandomMaterial(proj3);
    Projectile p = proj.GetComponent<Projectile>();
    Projectile p2 = proj2.GetComponent<Projectile>();
    Projectile p3 = proj3.GetComponent<Projectile>();
    p.player = playerObj;  // <-- 动态注入，永远不会丢失！
    p2.player = playerObj;
    p3.player = playerObj;

    Rigidbody rb = proj.GetComponent<Rigidbody>();
    Rigidbody rb2 = proj2.GetComponent<Rigidbody>();
    Rigidbody rb3 = proj3.GetComponent<Rigidbody>();
    if (rb != null)
        rb.AddForce(firePoint.forward * projectileSpeed, ForceMode.Impulse);
        // rb2.AddForce(firePoint2.forward * projectileSpeed, ForceMode.Impulse);
        // rb3.AddForce(firePoint.forward * projectileSpeed, ForceMode.Impulse);
        Vector3 dir2 = (playerTransform.position - firePoint2.position).normalized;
        Vector3 dir3 = (playerTransform.position - firePoint3.position).normalized;
        rb2.AddForce(dir2 * projectileSpeed, ForceMode.Impulse);
         rb3.AddForce(dir3 * projectileSpeed, ForceMode.Impulse);

        

}

   void ApplyRandomMaterial(GameObject proj)
{
    if (materialA == null || materialB == null) return;

    Renderer rend = proj.GetComponent<Renderer>();
    if (rend == null) return;

    // 随机选材质
    Material chosen = (UnityEngine.Random.value > 0.5f) ? materialA : materialB;

    // 使用新材质
    rend.material = chosen;
}

    void attack()
{
    RotateBodyHorizontal();
    RotateCannonVertical();
    ShootProjectile();
}

}
