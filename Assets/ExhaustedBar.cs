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
            StartCoroutine(ShowExhaustedText());
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
        yield return new WaitForSeconds(floatDuration);
        exhaustedText.SetActive(false);
        isShowing = false;
    }
    void OnDestroy()
    {
        if (_playerController != null)
        {
            _playerController.OnAttack -= HandleExhausted;
        }
        
    }
}
