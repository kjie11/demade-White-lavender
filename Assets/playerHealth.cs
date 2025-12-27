using System;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public event Action<float> OnTakeDamage; 
    public DamagePostProcess damagePostProcess;
    
    //notify UI manger to update pausemenu
    public static System.Action<float> OnHealthChanged;
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
         if (UIManager.Instance != null)
    {
        UIManager.Instance.RegisterPlayer(this);
    }
     Blood.OnHealthPackTaken += UpdateHealth;
    }

    // Update is called once per frame
    void Update()
    {
        fillImage.fillAmount = currentHealth / maxHealth; // UI HealthBar initialize
        if (currentHealth > 30)
        {
            damagePostProcess.recover();
        }
    }
    public void TakeDmage(float damage){
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            currentHealth -= damage;
            OnHealthChanged?.Invoke(currentHealth);
            takeDmageAnimation();
            OnTakeDamage?.Invoke(damage);
        }
        if (currentHealth <= 30)
        {
            damagePostProcess.TriggerDamageEffect();
        }
        
    }
    void Die(){
        animator.SetTrigger("Die");
        Debug.Log("player is die");
        SceneManager.LoadScene("Died");
    }
    //animation for lose heath count and reduce health bar
    void takeDmageAnimation(){
        // Debug.Log("player take damaged"+currentHealth);
        if (healthBar != null)
            healthBar.value = currentHealth;
    }

    //get the blood and update the health
    public void UpdateHealth(float healAmount)
{
    currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);

    OnHealthChanged?.Invoke(currentHealth);  
    Debug.Log("Heal received: " + healAmount);

    if (healthBar != null)
        healthBar.value = currentHealth;
}
void OnDestroy()
{
   Blood.OnHealthPackTaken -= UpdateHealth;
}

}
