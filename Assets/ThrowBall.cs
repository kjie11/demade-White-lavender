using UnityEngine;

public class ThrowBall : MonoBehaviour
{
    [Header("Ball Settings")]
    public float lifeTime = 2f;        // 自动销毁时间
    public float spinSpeed = 360f;     // 飞行时自旋角速度

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 5秒后自动销毁（避免场景堆积太多球）
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // 让球在飞行中自旋（看起来更有动感）
        // transform.Rotate(Vector3.right * spinSpeed * Time.deltaTime, Space.Self);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 如果撞到敌人 → 造成伤害
        enemyHealth enemy = collision.collider.GetComponent<enemyHealth>();
        if (enemy)
        {
            enemy.takeDamage(25f); // 你可以把伤害作为 public 变量
        }

        StaticEnemyHealth staticEnemy = collision.collider.GetComponent<StaticEnemyHealth>();
        if (staticEnemy)
        {
            staticEnemy.TakeDamage(25f);
        }

        // 撞到地面或墙后销毁（你也可以加爆炸）
        Destroy(gameObject);
    }

    // 💡 玩家抛球时调用这个方法来添加初始力
    public void Throw(Vector3 direction, float force)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
}
