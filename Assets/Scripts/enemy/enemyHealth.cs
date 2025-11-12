using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

public class enemyHealth : MonoBehaviour
{
    public enum EnemyType
    {
        white,//small
        Red,
        Yellow, //second
        Green,//boss
    }
    public EnemyType enemyType = EnemyType.Yellow;
    [Header("Health")]
    public float maxHealth = 100f;
    private float currentHealth;
    private Animator animator;
    private UnityEngine.AI.NavMeshAgent agent;

    [Header("UI Settings")]
    public Slider healthBar;

    [Header("Take damage Flash")]
    public float damageDuration=1f;
    public float flashInterval= 0.1f;// flash rate
    public int flashCount = 8;

    //Enemy drop parameters //notify enemyDrop script, and flash, drop health number animation
    public event Action<enemyHealth,float> OnTakeDamage; // event name is OntakeDmage, float is health drop number
    public event Action<enemyHealth> OnDeath;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
        //register takedamageEvent for UIManager
         if (UIManager.Instance != null)
    {
        UIManager.Instance.RegisterEnemy(this);
    }
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    // whenm enemy be attacked
    public void takeDamage(float damage){
        //loss health
        // takedamage animation
        //colldown time
        OnTakeDamage.Invoke(this,damage);
        if (currentHealth <= 0)
        {
            Die();
            Debug.Log("enemy die");
        }
        currentHealth -= damage;
        Debug.Log("enemy take damage" + currentHealth);
        if (healthBar != null)
            healthBar.value = currentHealth;
        

        if (currentHealth <= 0)
        {
            Die();
            
        }
        else
        {
            //when tage damage, enemy stop and flash for 1f
            StartCoroutine(StopForSeconds(1f));
               
        }
    }
    //stop the program
    private IEnumerator StopForSeconds(float seconds)
    {
        if (agent != null) agent.isStopped = true;
        yield return new WaitForSeconds(seconds);
        if (agent != null && currentHealth > 0) agent.isStopped = false;
    }
    //when take damage, flash
    void flash(){

    }

    void Die(){
        currentHealth = 0;
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // agent.isStopped = true;
        Debug.Log("in the die");
        OnDeath?.Invoke(this); 
        Destroy(gameObject); // enemy will disappear after 3f when died
    }
}
