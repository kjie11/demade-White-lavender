using System.Collections;
using TMPro;
using UnityEngine;

public class textPop : MonoBehaviour
{
    public TextMeshPro damageText;
    public AnimationCurve moveCurve;
    public AnimationCurve rotateCurve;
    public float duration = 1.0f;       
    public float moveHeight = 1.5f;     
    public float startRotation = 90f;
    public float endRotation = 0f;
    private Vector3 startPos;
   
   private enemyHealth enemyHealth; // use for receive takedamage invoke
    void Start()
    {
       enemyHealth = GetComponent<enemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnTakeDamage += PopDamageText;
            
        }
        startPos = transform.position;
        // StartCoroutine(Animate());
    }



    IEnumerator Animate()
    {
        float t = 0f;
        while (t < duration)
        {
            float n = t / duration;
            float moveY = moveCurve.Evaluate(n) * moveHeight;
            transform.position = startPos + Vector3.up * moveY;
            float angle = Mathf.Lerp(startRotation, endRotation, rotateCurve.Evaluate(n));
            transform.rotation = Quaternion.Euler(0, 0, angle);

            t += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }
    void PopDamageText(enemyHealth enemy, float damage)
    {
        damageText.text = damage.ToString();
        StartCoroutine(Animate());
    }
//     void OnDestroy()
// {
//     if (enemyHealth != null)
//         enemyHealth.OnTakeDamage -= PopDamageText;
// }
}
