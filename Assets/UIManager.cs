using System.Collections;
using System.Globalization;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("drop Item Settings")]
    public GameObject dropPrefab;
    [Range(0f, 1f)]
    public float dropChance = 0.25f; // 掉落概率 25%

    [Header("Explosion Effect")]

    public GameObject explosionPrefab;  //bomb core
    public GameObject coinPrefab;  
      public int minCoins = 3;
    public int maxCoins = 6;

    // [Header("knife tail effect")]
    // public TrailRenderer trail;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterPlayer(playerHealth player)
    {
        player.OnTakeDamage += HandlePlayerTakeDamage;
        // player.OnDeath += HandlePlayerTakeDamage;
    }

    public void UnregisterPlayer(playerHealth player)
    {
        player.OnTakeDamage -= HandlePlayerTakeDamage;
        // enemy.OnDeath -= HandleEnemyDeath;
    }
    // 注册/反注册敌人的事件
    public void RegisterEnemy(enemyHealth enemy)
    {
        enemy.OnTakeDamage += HandleEnemyTakeDamage;
        enemy.OnDeath += HandleEnemyDeath;
    }

    public void UnregisterEnemy(enemyHealth enemy)
    {
        enemy.OnTakeDamage -= HandleEnemyTakeDamage;
        enemy.OnDeath -= HandleEnemyDeath;
    }
    //static enemy
    public void RegisterStaticEnemy(StaticEnemyHealth enemy)
    {
        enemy.OnTakeDamage += HandleStaticEnemyTakeDamage;
        enemy.OnDeath += HandleStaticEnemyDeath;
    }

    private void HandleStaticEnemyDeath(StaticEnemyHealth enemy)
    {
         if (enemy == null) return;
        Vector3 pos = enemy.transform.position;
        StartCoroutine(HandleExplosionAndDrop(pos));
    }

    private void HandleStaticEnemyTakeDamage(StaticEnemyHealth enemy, float damage)
    {
        if (enemy != null)
        {
            Renderer rend = enemy.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                // StartCoroutine(FlashMaterial(rend, Color.red, 0.15f));
                StartCoroutine(FlashMaterial(rend, 0.6f, 2));
            }


        }
    }

    public void UnregisterStaticEnemy(StaticEnemyHealth enemy)
    {
        enemy.OnTakeDamage -= HandleStaticEnemyTakeDamage;
        enemy.OnDeath -= HandleStaticEnemyDeath;
    }

    private void HandlePlayerTakeDamage(float damage)
    {
        playerHealth player = FindObjectOfType<playerHealth>();
        if (player != null)
        {
            Renderer rend = player.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                // StartCoroutine(FlashMaterial(rend, Color.red, 0.15f));
                StartCoroutine(FlashMaterial(rend, 0.6f, 2));
            }


        }
    }

    // ✅ 敌人受伤时触发
    private void HandleEnemyTakeDamage(enemyHealth enemy, float damage)
    {
        Debug.Log($"Enemy took {damage} damage!");

        // 获取受伤敌人
        // enemyHealth enemy = FindObjectOfType<enemyHealth>();
        if (enemy != null)
        {
            Renderer rend = enemy.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                // StartCoroutine(FlashMaterial(rend, Color.red, 0.15f));
                StartCoroutine(FlashMaterial(rend, 0.6f, 2));
            }


        }

    }

   

     private void HandleEnemyDeath(enemyHealth enemy)
    {
        if (enemy == null) return;
        Vector3 pos = enemy.transform.position;
        StartCoroutine(HandleExplosionAndDrop(pos));
    }



    private IEnumerator FlashMaterial(Renderer rend, float duration = 0.6f, int flashCount = 3)
    {
        if (rend == null) yield break;

        Material mat = rend.material;
        Color originalColor = mat.color;
        Color flashColor1 = Color.white;
        Color flashColor2 = Color.black;

        float singleFlash = duration / (flashCount * 2f);

        for (int i = 0; i < flashCount; i++)
        {
            mat.color = flashColor1;
            yield return new WaitForSeconds(singleFlash);
            mat.color = flashColor2;
            yield return new WaitForSeconds(singleFlash);
        }

        mat.color = originalColor;
    }


    private void DropItem(Vector3 dropPosition)
    {
        //enmey bomb

        if (dropPrefab == null) return;

        // 掉落几率检测（整组是否掉落）
        float roll = Random.value;
        if (roll > dropChance)
        {
            Debug.Log("No item dropped.");
            return;
        }

        // ✅ 随机掉落数量：2 到 4 个（不包含上限）
        int dropCount = Random.Range(3, 6);

        // ✅ 控制掉落范围
        float minRadius = 1f;
        float maxRadius = 3f;

        for (int i = 0; i < dropCount; i++)
        {
            // 在环形区域内随机一个半径和角度
            float radius = Random.Range(minRadius, maxRadius);
            float angle = Random.Range(0f, Mathf.PI * 2f);

            // 计算在 XZ 平面的偏移
            float offsetX = Mathf.Cos(angle) * radius;
            float offsetZ = Mathf.Sin(angle) * radius;

            // 生成掉落位置（稍微抬高一点）
            Vector3 spawnPos = dropPosition + new Vector3(offsetX, 0.5f, offsetZ);

            // 随机旋转方向
            Quaternion randomRot = Quaternion.Euler(0, Random.Range(0, 360f), 0);

            // 生成掉落物
            GameObject drop = Instantiate(dropPrefab, spawnPos, randomRot);

            // ✅ 可选：添加一点弹跳力让掉落更自然
            Rigidbody rb = drop.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.up * Random.Range(1f, 2f), ForceMode.Impulse);
            }

            Debug.Log($"💎 Item {i + 1}/{dropCount} dropped at {spawnPos}");
        }
    }

    private IEnumerator HandleExplosionAndDrop(Vector3 pos)
    {
        // 💥 1️⃣ 生成你的爆炸球
        GameObject explosion = Instantiate(explosionPrefab, pos, Quaternion.identity);

        // 获取爆炸球脚本
        ExplosionCore core = explosion.GetComponent<ExplosionCore>();
        if (core == null)
        {
            Debug.LogWarning("Explosion prefab missing ExplosionCore!");
            yield break;
        }

        bool exploded = false;

        // 💡 监听爆炸结束事件
        core.OnExplosionEnd += () => exploded = true;

        // ⏳ 等待爆炸结束
        yield return new WaitUntil(() => exploded);

        int count = Random.Range(minCoins, maxCoins + 1);

        for (int i = 0; i < count; i++)
        {
            // ✅ 初始位置：敌人周围 0.5m 范围
            Vector2 randomOffset = Random.insideUnitCircle * 1f;
            Vector3 spawnPos = pos + new Vector3(randomOffset.x, 0.3f, randomOffset.y);

            GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);

            Rigidbody rb = coin.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // ✅ 随机喷射方向（略带上抛角度）
                Vector3 dir = new Vector3(randomOffset.x * Random.Range(1f, 1.6f), Random.Range(0.8f, 1.2f), randomOffset.y).normalized;

                // ✅ 向外 & 向上喷射力
                float force = Random.Range(1f, 3f);
                rb.AddForce(dir * force, ForceMode.Impulse);

                // ✅ 加一点随机旋转力，让金币飞旋
                rb.AddTorque(Random.insideUnitSphere * 4f, ForceMode.Impulse);

                // ✅ 可选：添加随机角度朝向
                coin.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
            }
        }
        Debug.Log($"💎 Dropped {count} coins after explosion.");
    }

//     public void StartTrail()
// {
//     if (trail != null)
//         trail.emitting = true;
// }

// public void StopTrail()
// {
//     if (trail != null)
//         trail.emitting = false;
// }
}
