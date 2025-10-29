using UnityEngine;
using UnityEngine.UI;

public class playerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;
    [Header("UI Settings")]
    public Slider healthBar;

    [Header("UI HealthBar")]
    public Image fillImage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth=maxHealth;
        animator = GetComponent<Animator>();
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    // Update is called once per frame
    void Update()
    {
        fillImage.fillAmount = currentHealth / maxHealth; // UI HealthBar initialize
    }
    public void TakeDmage(float damage){
        if (currentHealth <= 0) return;
         if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            currentHealth -= damage;
            
            takeDmageAnimation();
        }
    }
    void Die(){
        animator.SetTrigger("Die");
        Debug.Log("player is die");
    }
    //animation for lose heath count and reduce health bar
    void takeDmageAnimation(){
        // Debug.Log("player take damaged"+currentHealth);
        if (healthBar != null)
            healthBar.value = currentHealth;
    }
}
