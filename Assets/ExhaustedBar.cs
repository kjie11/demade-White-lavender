using System.Collections;
using JetBrains.Annotations;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using Unity.IntegerTime;
using UnityEngine;
using UnityEngine.UI;



public class ExhaustedBar : MonoBehaviour
{
    [Header("settings")]
    public float maxExhaust = 90f;
    public float attackCost = 30f;
    public float regenDelay = 1.5f; //after how many second it should start to recover
    public float regenPerSecond = 25f; //  recover persecond

    [Header("UI settings")]
    public UnityEngine.UI.Image exhaustedBar;
    private float currentExhaust;
    private float lastSpendTime = -999f; // ai to get nagative infenate 
    [Header("Player controller dependence")]
    public GameObject player; //it's player
    playerController _playerController;

    [Header("Exhausted UI")]
    public GameObject exhaustedText;

    public float floatDuration = 1.5f;
    private bool isShowing = false; // if exhasuted text is showm, not show again
    public AnimationCurve moveCurve;
    public AnimationCurve rotateCurve;
    public float moveHeight = 1.5f;     
    public float startRotation = 90f;
    public float endRotation = 0f;
    private Vector3 startPos;
    public GameObject exhastedPrefab;
    void Start()
    {
         exhaustedText.SetActive(false);
        currentExhaust = maxExhaust;
        if (exhaustedBar != null)
        {
            exhaustedBar.fillAmount = currentExhaust / maxExhaust;

        }
         _playerController = player.GetComponent<playerController>();
        if (_playerController != null)
        {
            _playerController.OnAttack += HandleExhausted;

        }
        startPos = exhaustedText.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastSpendTime >= regenDelay && currentExhaust < maxExhaust)
        {
            currentExhaust += regenPerSecond * Time.deltaTime;
            currentExhaust = Mathf.Min(currentExhaust, maxExhaust);
        }
        exhaustedBar.fillAmount = currentExhaust / maxExhaust;
    }

    //reduce exhausedBar
    void HandleExhausted()
    {
        currentExhaust -= attackCost;
        if (currentExhaust <= 0)
        {
            StartCoroutine(ExhaustAnimation());
        }
        currentExhaust = Mathf.Max(0f, currentExhaust); //ai
        if (exhaustedBar != null)
        {
            exhaustedBar.fillAmount = currentExhaust / maxExhaust;
        }

        lastSpendTime = Time.time;

    }
    //ai
    IEnumerator ShowExhaustedText()
    {
        if (isShowing) yield break;
        isShowing = true;
        exhaustedText.SetActive(true);
        // GameObject clone = Instantiate(exhastedPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(floatDuration);
        // Destroy(clone);




        exhaustedText.SetActive(false);
        isShowing = false;
    }
    IEnumerator ExhaustAnimation()
    {
         exhaustedText.SetActive(true);
         float t = 0f;
        while (t < floatDuration)
        {
            float n = t / floatDuration;
            float moveY = moveCurve.Evaluate(n) * moveHeight;
            exhaustedText.transform.position = startPos + Vector3.up * moveY;
            float angle = Mathf.Lerp(startRotation, endRotation, rotateCurve.Evaluate(n));
            exhaustedText.transform.rotation = Quaternion.Euler(0, 0, angle);

            t += Time.deltaTime;
            yield return null;
        }
        exhaustedText.SetActive(false);
    }
    void OnDestroy()
    {
        if (_playerController != null)
        {
            _playerController.OnAttack -= HandleExhausted;
        }
        
    }
}
