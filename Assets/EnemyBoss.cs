using UnityEngine;

public class EnemyBoss : enemyMove
{
    [Header("Jump Settings")]
    public float jumpForce = 5f;          // 跳跃力度
    public float jumpDuration = 0.8f;     // 跳跃过程持续时间
    private bool isJumping = false;       // 防止重复跳跃
    public float jumpCooldown = 5f;
    private float nextJumpTime = 0f;
    public float jumpMinDistance = 5f;  //when the distance less than 5, enemy walk follow player

    protected override void followPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 🟢 如果距离在5米以内，直接调用父类的“走路跟随”
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


        // ai
        Vector3 targetPos = playerTransform.position - playerTransform.forward * 1.0f;

        Vector3 startPos = transform.position;

        // 启动协程执行跳跃运动
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

            // 使用平滑插值 + 抛物线轨迹
            Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
            pos.y += Mathf.Sin(Mathf.PI * t) * jumpForce; // 形成弧线轨迹
            transform.position = pos;

            yield return null;
        }

        // 落地后恢复状态
        agent.Warp(targetPos);  // 确保在地面上
        agent.isStopped = false;
        isJumping = false;

    }
}
