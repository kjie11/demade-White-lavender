using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class enemyDrop : MonoBehaviour
{
    
    public GameObject[] dropItems;
    [Range(0f, 1f)] public float dropChance = 0.5f;
    private enemyHealth enemyHealth;
    [Header("damage number animation")]
    Vector3 originalPos; //damage number text position
    public GameObject damageNumberText;
    public TextMeshProUGUI damageTmp;
    public float floatDistance = 0.5f;
    public float floatDuration = 0.5f;
     public float stayDuration = 0.5f;  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        enemyHealth = GetComponent<enemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnTakeDamage += HandleTakeDamage;
            enemyHealth.OnDeath += HandleDeath;
        }
        damageNumberText.SetActive(false);
        originalPos = damageNumberText.transform.localPosition;

       
    }

    // Update is called once per frame
    void Update()
    {
        if (damageNumberText.activeSelf)
        {
            // 面向主相机
            damageNumberText.transform.LookAt(Camera.main.transform);
            // 反转 180 度，使文字不倒置
            damageNumberText.transform.Rotate(0, 180f, 0);
        }
    }
    void HandleTakeDamage(float damage)
    {
        Debug.Log("enemy get damage!!");
        // damageNumber animation
        damageNumberText.SetActive(true);
        damageTmp.text = damage.ToString();
        StartCoroutine(DamageNumberPopup());


    }
    System.Collections.IEnumerator DamageNumberPopup()
    {
        Vector3 startPos = originalPos;
        Vector3 endPos = startPos + Vector3.up * floatDistance;
        float elapsed = 0f;

        // 每帧上升并旋转
        while (elapsed < floatDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / floatDuration;

            // 上升
            damageNumberText.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            // 逆时针旋转 90°
            damageNumberText.transform.localRotation = Quaternion.Euler(0, 0, -90f * t);

            yield return null;
        }
        yield return new WaitForSeconds(stayDuration);
         // --- 第三阶段：淡出 + 缓慢上升 ---
    float fadeTime = 0.8f;          // 淡出持续时间（可调）
    float fadeElapsed = 0f;
    Vector3 fadeStartPos = damageNumberText.transform.localPosition;
    Vector3 fadeEndPos = fadeStartPos + Vector3.up * 1f; // 再上升一点

    Color startColor = damageTmp.color;

    while (fadeElapsed < fadeTime)
    {
        fadeElapsed += Time.deltaTime;
        float t = fadeElapsed / fadeTime;

        // 上升
        damageNumberText.transform.localPosition = Vector3.Lerp(fadeStartPos, fadeEndPos, t);

        // 渐隐
        Color c = startColor;
        c.a = Mathf.Lerp(1f, 0f, t);
        damageTmp.color = c;

        yield return null;
    }
        // 结束后复位并隐藏
        damageNumberText.transform.localPosition = originalPos;
        damageNumberText.transform.localRotation = Quaternion.identity;
        damageNumberText.SetActive(false);
        //damage number disappear


    }
    
    void ODestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnTakeDamage -= HandleTakeDamage;
            enemyHealth.OnDeath -= HandleDeath;
        }
    }
    void HandleDeath()
    {
        //爆炸效果
    }
}
