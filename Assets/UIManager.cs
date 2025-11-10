using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void RegisterPlayer(playerHealth player)
    {
        player.OnTakeDamage += HandlePlayerTakeDamage;
        // player.OnDeath += HandlePlayerTakeDamage;
    }

    public void UnregisterPlayer(playerHealth  player)
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

    private void HandlePlayerTakeDamage(float damage)
    {
        playerHealth player = FindObjectOfType<playerHealth>();
        if (player != null)
        {
            Renderer rend = player.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                // StartCoroutine(FlashMaterial(rend, Color.red, 0.15f));
                StartCoroutine(FlashMaterial(rend, 0.6f,2));
            }

           
         }
    }

    // ✅ 敌人受伤时触发
    private void HandleEnemyTakeDamage(enemyHealth enemy,float damage)
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
                StartCoroutine(FlashMaterial(rend, 0.6f,2));
            }

           
         }
    }

    // ✅ 敌人死亡时触发
    private void HandleEnemyDeath()
    {
        Debug.Log("Enemy died! Show effects or play animation here.");
        // 获取dead enemy
        enemyHealth enemy = FindObjectOfType<enemyHealth>();
        if (enemy != null)
        {
            Renderer rend = enemy.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
                // StartCoroutine(FlashMaterial(rend, Color.red, 0.15f));
                StartCoroutine(FlashMaterial(rend, 0.4f,6));
            }

           
         }
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
}
