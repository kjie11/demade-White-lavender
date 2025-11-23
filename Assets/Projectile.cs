using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float damage = 10f;
    public float lifeTime = 5f;
    public float destroyDelay = 0.5f;

    private bool hasCollided = false;
    public string playerTag = "Player";
    public GameObject player;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }



    private void OnTriggerEnter(Collider other)
{
    if (hasCollided) return;
    hasCollided = true;
      Debug.Log($"Projectile first hit: {other.name}, Tag: {other.tag}");

    if (other.CompareTag(playerTag))
    {
        playerHealth ph = player.GetComponent<playerHealth>();
        if (ph != null)
        {
            ph.TakeDmage(damage);
        }
        Debug.Log("PROJECTILE NO DELAY");
        Destroy(gameObject);
    }
    else
    {
        Debug.Log("PROJECTILE DELAY");
        Destroy(gameObject, destroyDelay);
    }
}
}
