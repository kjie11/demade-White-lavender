using System.Collections;
using UnityEngine;
using UnityEngine.AI;
public class enemyMove : MonoBehaviour
{
    protected NavMeshAgent agent;
    public Transform playerTransform;
    [Header("Animation")]
    protected Animator animator;
    [Header("Movement")]
    public float walkSpeed = 1.5f;
    [Header("Patrol Settings")]
    public float patrolRadius = 5f;
    protected Vector3 patrolCenter;
    private Vector3 patrolPoint; // ramdom get the point, make the enemy move around in the patrol area
    public float patrolWaitTime = 1f; // patrol and wait a time to change patrol point
    private float waitTimer = 0f;
    public GameObject anchor;
    [Header("Attack")]
    public float attackDistance = 2f;
    public float attackCooldown = 3f;
    private float nextAttackTime = 0f;
    public float attackDamageCount = 30f;
    [Header("Audio")]
    public AudioClip alertSound;
    public float alertSoundCooldown = 5f; // Avoid alert effect too frequently
    private float lastAlertTime = -999f;
    private bool playerInRange = false;   
    private AudioSource audioSource;      
    protected Vector3 offset = Vector3.back;
    protected virtual void Awake()
    {
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerTransform = player.transform;
        }
    }
    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        patrolCenter = transform.position;
        agent.autoBraking = false;
        patrolPoint = patrolCenter;
        anchor.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }
    protected virtual void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
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
    // ask ai how to limit the frequent playback of sound effects
    protected virtual void OnPlayerEnterRange()
    {
        if (alertSound != null && Time.time - lastAlertTime > alertSoundCooldown)
        {
            audioSource.PlayOneShot(alertSound);
            lastAlertTime = Time.time;
        }
    }
    protected virtual void followPlayer()
    {
        anchor.SetActive(true);
        SafeSetTrigger("Follow");
        agent.destination = playerTransform.position + offset;
    }

    //ai: how to ise the attack coroutine and why use coroutine
    private IEnumerator AttackCoroutine()
    {
        // ask ai how to wait for the exact moment when the attack animation actually hits.
        yield return new WaitForSeconds(1f);
        playerHealth ph = playerTransform.GetComponent<playerHealth>();
        if (ph != null)
        {
            ph.TakeDmage(attackDamageCount); 
        }
        else
        {
            Debug.Log("player had no player health script");
        }
        yield return new WaitForSeconds(0.2f);
        Invoke(nameof(ResumeMoveAfterAttack), 0.6f);
        agent.isStopped = false;
    }
    protected virtual void attack()
    {
        //enemy stop and face to player
        agent.isStopped = true;
        Vector3 look = playerTransform.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(look);
        }
        animator.ResetTrigger("Attack");
        animator.ResetTrigger("Attack2");
        int randomAttack = Random.Range(0, 2);

        if (randomAttack == 0)
        {
            SafeSetTrigger("Attack");
        }
        else
        {
            SafeSetTrigger("Attack");
        }
        StartCoroutine(AttackCoroutine());
    }
    void ResumeMoveAfterAttack()
    {
        agent.isStopped = false;
    }
    void Patrol()
    {
        anchor.SetActive(false);
        agent.destination = patrolPoint;
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
                SetNewPatrolPoint();
                waitTimer = 0f;
            }
        } //ai:to refne the patrol logic, enemy short wait after the patrol to one point.
    }
    protected void SetNewPatrolPoint()
    {
        Vector3 randomPoint = Random.insideUnitSphere * patrolRadius; // ai: how to get random point
        randomPoint += patrolCenter;
        NavMeshHit hit; //ai
        if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
        {
            patrolPoint = hit.position;
            agent.speed = walkSpeed;
            agent.destination = patrolPoint;
        }
    }
    protected void SafeSetTrigger(string triggerName)
    {
        if (animator != null)
            animator.SetTrigger(triggerName);
    }
}
