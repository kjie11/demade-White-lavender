// using UnityEngine;
// using UnityEngine.AI;

// public class Enemy3Move : MonoBehaviour
// {
//     [Header("Detection")]
//     public Transform player;
//     public float triggerRadius = 6f;

//     [Header("Patrol")]
//     public float patrolRadius = 4f;
//     public float patrolWaitTime = 1.5f;

//     [Header("Movement")]
//     public float moveSpeed = 2f;

//     private NavMeshAgent agent;
//     private bool isActive = false;

//     private Vector3 patrolCenter;
//     private float waitTimer;

//     void Start()
//     {
//         agent = GetComponent<NavMeshAgent>();
//         agent.speed = moveSpeed;
//         agent.enabled = false; // 初始不动（地下 / 未激活）

//         patrolCenter = transform.position;
//     }

//     void Update()
//     {
//         if (!isActive)
//         {
//             CheckPlayerTrigger();
//             return;
//         }

//         Patrol();
//     }

//     // 玩家进入范围 → 激活
//     void CheckPlayerTrigger()
//     {
//         if (Vector3.Distance(transform.position, player.position) <= triggerRadius)
//         {
//             ActivateEnemy();
//         }
//     }

//     void ActivateEnemy()
//     {
//         isActive = true;
//         agent.enabled = true;
//         agent.Warp(transform.position); 

//         SetNewPatrolPoint();
//     }


//     void Patrol()
//     {
//         if (agent.pathPending) return;

//         if (agent.remainingDistance <= 0.3f)
//         {
//             waitTimer += Time.deltaTime;
//             if (waitTimer >= patrolWaitTime)
//             {
//                 SetNewPatrolPoint();
//                 waitTimer = 0f;
//             }
//         }
//     }

//     void SetNewPatrolPoint()
//     {
//         Vector3 randomPoint = Random.insideUnitSphere * patrolRadius;
//         randomPoint.y = 0f;
//         randomPoint += patrolCenter;

//         if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
//         {
//             agent.SetDestination(hit.position);
//         }
//     }


//     void OnDrawGizmosSelected()
//     {
//         Gizmos.color = Color.red;
//         Gizmos.DrawWireSphere(transform.position, triggerRadius);

//         Gizmos.color = Color.yellow;
//         Gizmos.DrawWireSphere(patrolCenter, patrolRadius);
//     }
// }



using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy3Move : enemyMove
{
    [Header("Enemy3 Settings")]
    public float triggerRadius = 6f;   // 玩家靠近时激活
    private bool isActive = false;     // 是否激活

    private Vector3 spawnPos;

    protected override void Start()
{
    base.Start();   // ✔ 运行父类 enemyMove 的初始化

    spawnPos = transform.position;

    agent.enabled = false;   // ✔ 禁用 NavMeshAgent，防止回弹到地面
}


    void Update()
    {
        // 🟡 未激活：不执行父类 Update，只等待触发
        if (!isActive)
        {
            float distance = Vector3.Distance(spawnPos, playerTransform.position);
            if (distance <= triggerRadius)
            {
                ActivateEnemy();
            }
            return;
        }

        // 🟢 已激活：使用父类 enemyMove 的 AI（巡逻、追随、攻击）
        base.Update();
    }

    

    void ActivateEnemy()
    {
        isActive = true;
        // agent.enabled = true;
        agent.enabled = true;
        agent.Warp(spawnPos);
        StartCoroutine(RiseUp());

        // patrolCenter = spawnPos;
        // SetNewPatrolPoint();
    }
    IEnumerator RiseUp()
{
    float duration = 1f;
    float t = 0f;
    Vector3 start = transform.position;
    Vector3 end = spawnPos;

    while (t < duration)
    {
        t += Time.deltaTime;
        transform.position = Vector3.Lerp(start, end, t / duration);
        yield return null;
    }

    // 开始巡逻
    patrolCenter = spawnPos;
    SetNewPatrolPoint();
}

    // ===== 覆盖追随：无动画 =====
    protected override void followPlayer()
    {
        anchor.SetActive(true);
        agent.destination = playerTransform.position + offset;
    }

    // ===== 覆盖攻击：无动画 =====
    // protected override void attack()
    // {
    //     agent.isStopped = true;

    //     Vector3 look = playerTransform.position - transform.position;
    //     look.y = 0f;
    //     transform.rotation = Quaternion.LookRotation(look);

    //     playerHealth ph = playerTransform.GetComponent<playerHealth>();
    //     if (ph != null)
    //     {
    //         ph.TakeDmage(attackDamageCount);
    //     }

    //     Invoke(nameof(ResumeMoveAfterAttack3), 0.3f);
    // }

    protected override void attack()
{
    if (!gameObject.activeInHierarchy) return;

    agent.isStopped = true;

    StartCoroutine(JumpAttack());
}

IEnumerator JumpAttack()
{
    agent.enabled = false; // 暂时禁用NavMeshAgent避免冲突

    Vector3 start = transform.position;

    // 目标点（跳到玩家脚下前一点位置）
    Vector3 direction = (playerTransform.position - transform.position).normalized;
    Vector3 end = playerTransform.position - direction * 0.5f; // 怪物不会“撞进玩家身体”

    float height = 1.5f;    // 跳跃高度，可调大/调小
    float duration = 0.35f; // 跳跃时间

    float t = 0f;

    while (t < 1f)
    {
        t += Time.deltaTime / duration;

        // 水平移动
        Vector3 horizontal = Vector3.Lerp(start, end, t);

        // 垂直抛物线 (Parabola)
        float y = Mathf.Sin(Mathf.PI * t) * height;

        transform.position = new Vector3(horizontal.x, start.y + y, horizontal.z);

        yield return null;
    }
    
    float landingDelay = 0.25f;
    yield return new WaitForSeconds(landingDelay);

    // 落地后造成伤害
    DealDamageToPlayer();

    // 恢复NavMeshAgent
    agent.enabled = true;
    agent.Warp(transform.position); 
    agent.isStopped = false;

    yield return new WaitForSeconds(0.2f);
}
void DealDamageToPlayer()
{
    playerHealth ph = playerTransform.GetComponent<playerHealth>();
    if (ph != null)
        ph.TakeDmage(attackDamageCount);
}

    void ResumeMoveAfterAttack3()
    {
        agent.isStopped = false;
    }
}
