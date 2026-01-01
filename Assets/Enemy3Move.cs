

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Enemy3Move : enemyMove
{
    [Header("Enemy3 Settings")]
    public float triggerRadius = 6f;
    private bool isActive = false;
    private Vector3 spawnPos;

    protected override void Start()
    {
        base.Start();

        spawnPos = transform.position;

        agent.enabled = false;
    }


    void Update()
    {

        if (!isActive)
        {
            float distance = Vector3.Distance(spawnPos, playerTransform.position);
            if (distance <= triggerRadius)
            {
                ActivateEnemy();
            }
            return;
        }


        base.Update();
    }



    void ActivateEnemy()
    {
        isActive = true;

        agent.enabled = true;
        agent.Warp(spawnPos);
        StartCoroutine(RiseUp());

    }
    IEnumerator RiseUp()
    {
        float duration = 0.5f;
        float t = 0f;
        Vector3 start = transform.position;
        Vector3 end = spawnPos;

        while (t < duration)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start, end, t / duration);
            yield return null;
        }


        patrolCenter = spawnPos;
        SetNewPatrolPoint();
    }

    protected override void OnPlayerEnterRange()
    {
        StartCoroutine(DelayedEnterRange());
    }
    private IEnumerator DelayedEnterRange()
    {
        yield return new WaitForSeconds(0.5f);

        base.OnPlayerEnterRange();
    }


    protected override void attack()
    {
        if (!gameObject.activeInHierarchy) return;

        agent.isStopped = true;

        StartCoroutine(JumpAttack());
    }

    protected override void followPlayer()
    {

        anchor.SetActive(true);



    }

    //ai： how to make jumping more responsive and how to implement parabolic motion.
    IEnumerator JumpAttack()
    {
        agent.enabled = false;
        Vector3 start = transform.position;
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Vector3 end = playerTransform.position - direction * 0.5f;
        float height = 1.5f;
        float duration = 0.35f;
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            Vector3 horizontal = Vector3.Lerp(start, end, t);
            float y = Mathf.Sin(Mathf.PI * Mathf.Pow(t, 0.7f)) * height;
            transform.position = new Vector3(horizontal.x, start.y + y, horizontal.z);
            yield return null;
        }
        float landingDelay = 0.25f;
        yield return new WaitForSeconds(landingDelay);
        DealDamageToPlayer();
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
