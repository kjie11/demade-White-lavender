using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
public class enemyMove : MonoBehaviour
{
    protected NavMeshAgent agent;
    public Transform playerTransform;
    [Header("Animation")]
    protected Animator animator;
    

    [Header("Movement")]
    public float walkSpeed= 1.5f;

    [Header("Patrol Settings")]
    public float patrolRadius=5f;
    protected Vector3 patrolCenter;
    private Vector3 patrolPoint; // ramdom get the point, make the enemy move around in the patrol area
     public float patrolWaitTime = 1f; // patrol and wait a time to change patrol point
    private float waitTimer = 0f;
    public GameObject anchor;

    [Header("Attack")]
    public float attackDistance=2f;
    public float attackCooldown = 3f; 
    private float nextAttackTime = 0f;
    public float attackDamageCount=30f;

    [Header("Audio")]
    public AudioClip alertSound;          
    public float alertSoundCooldown = 5f; // 防止频繁播放
    private float lastAlertTime = -999f;
    private bool playerInRange = false;   // 用来检测进入/离开范围状态
    private AudioSource audioSource;      // 声音播放器
    


    protected Vector3 offset=Vector3.back;
    protected virtual void Awake()
{
    if (playerTransform == null)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
    }
}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
         animator=GetComponent<Animator>();
        agent=GetComponent<NavMeshAgent>();
        patrolCenter=transform.position;
        
        agent.autoBraking=false;
        patrolPoint = patrolCenter;
        anchor.SetActive(false);
         // ✅ 初始化 AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        // 🎯 检测是否刚进入巡逻范围
        bool isInRangeNow = distanceToPlayer < patrolRadius;
        if (isInRangeNow && !playerInRange)
        {
            OnPlayerEnterRange();
        }
        playerInRange = isInRangeNow;

        if (distanceToPlayer < attackDistance && Time.time >= nextAttackTime)
        {
            attack();
            nextAttackTime = Time.time + attackCooldown;
        }
        else if (distanceToPlayer < patrolRadius)
        {
            followPlayer();
        }
        else
        {
            Patrol();
        }

    }
     // 🟡 当玩家第一次进入巡逻半径
    void OnPlayerEnterRange()
    {
        if (alertSound != null && Time.time - lastAlertTime > alertSoundCooldown)
        {
            audioSource.PlayOneShot(alertSound);
            lastAlertTime = Time.time;
        }
        Debug.Log("⚠️ Player entered patrol range!");
    }

   
   protected virtual void followPlayer(){
        // animator.SetTrigger("Follow");
         anchor.SetActive(true);
        SafeSetTrigger("Follow");
        agent.destination=playerTransform.position +offset;


    }
    protected virtual void attack(){
        //enemy stop and face to player
        agent.isStopped = true;
        Vector3 look = playerTransform.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.001f) {
            transform.rotation = Quaternion.LookRotation(look);
        }
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Attack2");
         int randomAttack = Random.Range(0, 2);

        if (randomAttack == 0)
        {
            // animator.SetTrigger("Attack");
            SafeSetTrigger("Attack");
        }
        else
        {
            // animator.SetTrigger("Attack2");
            SafeSetTrigger("Attack");
        }
        // player lose health
        playerHealth ph= playerTransform.GetComponent<playerHealth>();
        if(ph!=null){
            ph.TakeDmage(attackDamageCount); //todo : random damage account
        }
        else{
            Debug.Log("player had no player health script");
        }

        Invoke(nameof(ResumeMoveAfterAttack), 0.6f);
        }


    void ResumeMoveAfterAttack(){
        agent.isStopped=false;
    }
    void Patrol(){
        // animator.SetTrigger("Walking");
         anchor.SetActive(false);
        agent.destination=patrolPoint;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
                SetNewPatrolPoint();
                waitTimer = 0f;
            }
        } //ai

    }

    protected void SetNewPatrolPoint(){
        Vector3 randomPoint=Random.insideUnitSphere*patrolRadius; // ai: how to get random point
        randomPoint+=patrolCenter;
        NavMeshHit hit; //ai
        if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
            agent.speed = walkSpeed;
            agent.destination = patrolPoint;
        }
    }

    //if enmey has not animator
    protected void SafeSetTrigger(string triggerName)
{
    if (animator != null)
        animator.SetTrigger(triggerName);
}
}
