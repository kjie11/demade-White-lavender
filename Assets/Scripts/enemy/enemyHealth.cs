using UnityEngine;
using System.Collections;
using UnityEngine.AI;

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

    [Header("Take damage Flash")]
    public float damageDuration=1f;
    public float flashInterval= 0.1f;// flash rate
    public int flashCount=8;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
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
