using UnityEngine;

public class EnemyBoss : enemyMove
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;
    public float jumpDuration = 0.8f;
    private bool isJumping = false;
    public float jumpCooldown = 5f;
    private float nextJumpTime = 0f;
    public float jumpMinDistance = 5f;  //when the distance less than 5, enemy walk follow player
    protected override void followPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= jumpMinDistance)
        {
            base.followPlayer();
            return;
        }
        if (Time.time < nextJumpTime) return;
        if (isJumping) return;
        nextJumpTime = Time.time + jumpCooldown;
        isJumping = true;
        agent.isStopped = true;
        anchor.SetActive(true);
        // Ask ai how to use Coroutine to implement animation while actions
        Vector3 targetPos = playerTransform.position - playerTransform.forward * 1.0f;

        Vector3 startPos = transform.position;
        StartCoroutine(JumpToPlayer(startPos, targetPos));
    }
    protected override void attack()
    {
        // when jumping, enemy cannot attack
        if (isJumping) return;
        base.attack();
    }
    private System.Collections.IEnumerator JumpToPlayer(Vector3 startPos, Vector3 targetPos)
    {
        float elapsed = 0f;
        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;
            // Asked AI how to use smooth interpolation and parabolic motion
            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
            pos.y += Mathf.Sin(Mathf.PI * t) * jumpForce; // parabolic motion
            transform.position = pos;
            yield return null;
        }
        // ask AI how to make sure enemy is on the ground correctly
        agent.Warp(targetPos);
        agent.isStopped = false;
        isJumping = false;
    }
}
