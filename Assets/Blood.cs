using System;
using UnityEngine;

public class Blood : MonoBehaviour
{
     public float healAmount = 20f;

    
    public static event Action<float> OnHealthPackTaken;

     [Header("Sound Settings")]
    public AudioClip pickupSound;        
    public float volume = 1.0f;          
    private AudioSource audioSource;
     private void Start()
    {
        // 自动添加 AudioSource
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")||other.CompareTag("StaticEnemyAttack"))
        {
            
           Pickup();

           
        }
    }
    private void OnCollisionEnter(Collision collision)
{
    if (collision.collider.CompareTag("Player"))
    {
         Pickup();

    }
}

private void Pickup()
    {
        // 触发回血事件
        OnHealthPackTaken?.Invoke(healAmount);

        // 播放音效
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position, volume);
            
        }

        // 销毁血包本体
        Destroy(gameObject);
    }

}
