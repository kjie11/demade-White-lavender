using System.Collections;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [Header("吸附范围设置")]
    public float attractRadius = 3f;       // 开始被吸引的距离
    public float pickupDistance = 0.12f;    // 实际拾取距离
    public float moveSpeed = 5f;           // 飞向玩家的速度
     private Rigidbody rb;

    [Header("金币拾取效果")]
    // public GameObject pickupEffect;
    public AudioClip pickupSound;

    private Transform player;              // 玩家位置
    private bool isAttracted = false;      // 是否开始飞向玩家
    private bool collected = false;

    //show find coin canvas
    public GameObject findCoinCanvasPrefab;

    public static event System.Action OnCoinCollected; //notify UI

    void Start()
    {
        
        rb = GetComponent<Rigidbody>();
        // 找到场景中的玩家（需要确保玩家带有"Player"标签）
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null || collected) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // // 🧲 距离小于吸附半径时开始飞向玩家
        // if (distance <= attractRadius)
        // {
        //     isAttracted = true;
        // }

        // 🌀 如果开始被吸引，向玩家移动
        // if (isAttracted)
        // {
//             transform.position = Vector3.Lerp(
//     transform.position,
//     player.position,
//     moveSpeed * Time.deltaTime
// );
            Debug.Log("distance" +distance);

            // 当接近到指定距离时触发拾取
            if (distance <= pickupDistance)
            {
                CollectCoin();
            }
        // }
    }

    private void CollectCoin()
    {
        if (collected) return;
        collected = true;

        // ✅ 播放特效
        // if (pickupEffect != null)
        //     Instantiate(pickupEffect, transform.position, Quaternion.identity);

        // ✅ 播放音效
        if (pickupSound != null)
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
// ✅ 实例化 Canvas 提示（显示3秒）
        if (findCoinCanvasPrefab != null)
        {
            GameObject popup = Instantiate(findCoinCanvasPrefab, Vector3.zero, Quaternion.identity);
            
            // 如果这个 UI 是世界空间 Canvas，可以放在玩家上方
            // popup.transform.position = player.position + Vector3.up * 2f;

            // 如果是屏幕UI，可以让它成为Canvas的子物体
            Canvas mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas != null)
                popup.transform.SetParent(mainCanvas.transform, false);
            OnCoinCollected?.Invoke();
            Destroy(popup, 1f); 
        }

        // ✅ 销毁金币对象
        Destroy(gameObject);
    }

    // private IEnumerator ShowCanvasThenHide()
    // {
    //     if (findCoinCanvas != null)
    //     {
    //         findCoinCanvas.SetActive(true);
    //         yield return new WaitForSeconds(3f);  // ⏳ 显示 3 秒
    //         findCoinCanvas.SetActive(false);
    //     }
    // }
    private void OnCollisionEnter(Collision collision)
{
    // 检测是否落地（地面可加标签"Ground"）
    if (collision.gameObject.CompareTag("Ground"))
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }
}
}
