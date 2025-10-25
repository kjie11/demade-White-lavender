using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.UI;

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
    public int flashCount=8;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // whenm enemy be attacked
    public void takeDamage(float damage){
        //loss health
        // takedamage animation
        //colldown time
        if (currentHealth <= 0) return; // if death
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
        animator.SetTrigger("Die");
        agent.isStopped = true;
        Destroy(gameObject, 3f); // enemy will disappear after 3f when died
    }
}
