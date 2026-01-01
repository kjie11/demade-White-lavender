using System.Collections;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("drop Item Settings")]
    public GameObject dropPrefab;
    [Range(0f, 1f)]
    public float dropChance = 0.25f;

    [Header("Explosion Effect")]
    public GameObject explosionPrefab;
    public GameObject coinPrefab;
    public int minCoins = 3;
    public int maxCoins = 6;

    [Header("Coin UI")]
    public TextMeshProUGUI coinText;
    private int coinCount = 0;
    private Coroutine coinDisplayRoutine;

    [Header("pause mene")]
    public TextMeshProUGUI healthPause;
    public TextMeshProUGUI staminaPause;
    public TextMeshProUGUI coinPause;

    void Awake()
    {
        Instance = this;
    }
    //receive events
    private void OnEnable()
    {
        CoinPickup.OnCoinCollected += AddCoin;
        ExhaustedBar.OnStaminaChanged += UpdateStamina;
        playerHealth.OnHealthChanged += UpdateHealth;
    }

    //ai
    private void UpdateStamina(float obj)
    {
        obj = Mathf.Max(0f, obj);
        staminaPause.text = obj.ToString("0");
    }
    private void UpdateHealth(float obj)
    {
        obj = Mathf.Max(0f, obj);
        healthPause.text = obj.ToString("0");
    }

    private void OnDisable()
    {
        CoinPickup.OnCoinCollected -= AddCoin;
    }

    private void AddCoin()
    {
        coinCount++;
        coinText.text = "Coins: " + coinCount;
        coinText.gameObject.SetActive(true);
        coinPause.text = coinCount.ToString();
        if (coinDisplayRoutine != null)
            StopCoroutine(coinDisplayRoutine);
        coinDisplayRoutine = StartCoroutine(HideCoinUIAfterDelay());
    }
    private IEnumerator HideCoinUIAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        coinText.gameObject.SetActive(false);
    }

    public void RegisterPlayer(playerHealth player)
    {
        player.OnTakeDamage += HandlePlayerTakeDamage;
    }

    public void UnregisterPlayer(playerHealth player)
    {
        player.OnTakeDamage -= HandlePlayerTakeDamage;
    }
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
                StartCoroutine(FlashMaterial(rend, 0.6f, 2));
            }
        }
    }

    //how to get the enemy material and make material flash
    private void HandleEnemyTakeDamage(enemyHealth enemy, float damage)
    {
        if (enemy != null)
        {
            Renderer rend = enemy.GetComponentInChildren<Renderer>();
            if (rend != null)
            {
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

    //ai: how to make the material flash
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

        // ai: how to drop item with a random value
        float roll = Random.value;
        if (roll > dropChance)
        {
            return;
        }

        int dropCount = Random.Range(3, 6);
        float minRadius = 1f;
        float maxRadius = 3f;

        //ai: how to drop item at a random positio
        for (int i = 0; i < dropCount; i++)
        {
            // generate drop item at random position
            float radius = Random.Range(minRadius, maxRadius);
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float offsetX = Mathf.Cos(angle) * radius;
            float offsetZ = Mathf.Sin(angle) * radius;
            Vector3 spawnPos = dropPosition + new Vector3(offsetX, 0.5f, offsetZ);
            Quaternion randomRot = Quaternion.Euler(0, Random.Range(0, 360f), 0);

            GameObject drop = Instantiate(dropPrefab, spawnPos, randomRot);

            Rigidbody rb = drop.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(Vector3.up * Random.Range(1f, 2f), ForceMode.Impulse);
            }
        }
    }

    //ai: How to create an explosion effect with small projectiles emitted in parabolic trajectories
    private IEnumerator HandleExplosionAndDrop(Vector3 pos)
    {
        GameObject explosion = Instantiate(explosionPrefab, pos, Quaternion.identity);
        ExplosionCore core = explosion.GetComponent<ExplosionCore>();
        if (core == null)
        {
            yield break;
        }

        bool exploded = false;
        core.OnExplosionEnd += () => exploded = true;
        yield return new WaitUntil(() => exploded);
        int count = Random.Range(minCoins, maxCoins + 1);
        for (int i = 0; i < count; i++)
        {
            Vector2 randomOffset = Random.insideUnitCircle * 1f;
            Vector3 spawnPos = pos + new Vector3(randomOffset.x, 0.3f, randomOffset.y);
            GameObject coin = Instantiate(coinPrefab, spawnPos, Quaternion.identity);
            Rigidbody rb = coin.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = new Vector3(randomOffset.x * Random.Range(1f, 1.6f), Random.Range(0.8f, 1.2f), randomOffset.y).normalized;
                float force = Random.Range(1f, 3f);
                rb.AddForce(dir * force, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * 4f, ForceMode.Impulse);
                coin.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);
            }
        }
    }
}
