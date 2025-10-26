using UnityEngine;

public class FootContactEffect : MonoBehaviour
{
    [Header("Ground Check Settings")]
    public float checkDistance = 0.12f;          
    public LayerMask groundLayer;               
    public GameObject circleEffect;             

    private bool isOnGround = false;            

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("checking distance");
    }

    // Update is called once per frame
    void Update()
    {
        bool hitGround = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, checkDistance, groundLayer);

        // 如果刚刚踩到地面
        if (hitGround )
        {
             Debug.Log($"hit, 距离 = {hit.distance:F3}");
            isOnGround = true;
            if (circleEffect != null)
                circleEffect.SetActive(true);
        }

        // 如果刚刚离开地面
        if (!hitGround)
        {
             Debug.Log($"no hit, 距离 = {hit.distance:F3}");
            isOnGround = false;
            if (circleEffect != null)
                circleEffect.SetActive(false);
        }
    }
    
}
