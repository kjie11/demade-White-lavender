using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class StaticEnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("UI Settings")]
    public Slider healthBar;

    [Header("Hit Flash Settings")]
    public float hitStopTime = 0.1f;
    public float flashInterval = 0.1f;
    public int flashCount = 5;

    // 事件回调
    public event Action<StaticEnemyHealth, float> OnTakeDamage;
    public event Action<StaticEnemyHealth> OnDeath;

    private Renderer[] renderers;  // 用于闪烁阴影
    private Color[] originalColors;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        // 获取所有 Renderers 用于闪烁
        renderers = GetComponentsInChildren<Renderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                originalColors[i] = renderers[i].material.color;
        }

        // 注册到 UIManager（如果你项目有）
        if (UIManager.Instance != null)
            UIManager.Instance.RegisterStaticEnemy(this);
    }

    // 外部调用：敌人受到伤害
    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        // 更新血条
        if (healthBar != null)
            healthBar.value = currentHealth;

        // 事件回调
        OnTakeDamage?.Invoke(this, damage);

        Debug.Log($"{name} take damage: {currentHealth}");

        // Hit Flash
        StartCoroutine(FlashDamageEffect());

        // 死亡判定
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashDamageEffect()
    {
        // 停顿时间（硬直）
        yield return new WaitForSeconds(hitStopTime);

        for (int i = 0; i < flashCount; i++)
        {
            SetAllRenderersColor(Color.white);
            yield return new WaitForSeconds(flashInterval);

            RestoreRendererColors();
            yield return new WaitForSeconds(flashInterval);
        }
    }

    private void SetAllRenderersColor(Color c)
    {
        foreach (var r in renderers)
        {
            if (r.material.HasProperty("_Color"))
                r.material.color = c;
        }
    }

    private void RestoreRendererColors()
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
                renderers[i].material.color = originalColors[i];
        }
    }

    private void Die()
    {
        Debug.Log($"{name} died!");

        OnDeath?.Invoke(this);

        Destroy(gameObject);
    }
}
