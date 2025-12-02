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
    public float moveHeight = 2.5f;     
    public float startRotation = 90f;
    public float endRotation = 0f;
    private Vector3 startPos;
    public GameObject exhastedPrefab;

    //notify UI manger to update pausemenu
    public static System.Action<float> OnStaminaChanged;

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
            OnStaminaChanged?.Invoke(currentExhaust);
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
        exhaustedText.transform.position = _playerController.transform.position + new Vector3(0f, 0.5f, 0f);
        // GameObject clone = Instantiate(exhastedPrefab, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(floatDuration);
        // Destroy(clone);




        exhaustedText.SetActive(false);
        isShowing = false;
    }
    // IEnumerator ExhaustAnimation()
    // {
    //      exhaustedText.SetActive(true);
    //      float t = 0f;
    //     while (t < floatDuration)
    //     {
    //         float n = t / floatDuration;
    //         float moveY = moveCurve.Evaluate(n) * moveHeight;
    //         exhaustedText.transform.position = startPos + Vector3.up * moveY;
    //         float angle = Mathf.Lerp(startRotation, endRotation, rotateCurve.Evaluate(n));
    //         exhaustedText.transform.rotation = Quaternion.Euler(0, 0, angle);

    //         t += Time.deltaTime;
    //         yield return null;
    //     }
    //     exhaustedText.SetActive(false);
    // }
    IEnumerator ExhaustAnimation()
{
    exhaustedText.SetActive(true);
    float t = 0f;

    while (t < floatDuration)
    {
        float n = t / floatDuration;

        // 基于玩家位置更新
        Vector3 basePos = _playerController.transform.position;
        float moveY = moveCurve.Evaluate(n) * moveHeight;
        exhaustedText.transform.position = basePos + new Vector3(0f, moveY, 0f);

        // 先让文字朝向摄像机
        Vector3 camDir = exhaustedText.transform.position - Camera.main.transform.position;
        Quaternion lookRot = Quaternion.LookRotation(camDir);

        // 在看向摄像机的基础上，增加Z轴旋转动画
        float angle = Mathf.Lerp(startRotation, endRotation, rotateCurve.Evaluate(n));
        Quaternion spinRot = Quaternion.Euler(0, 0, angle);

        exhaustedText.transform.rotation = lookRot * spinRot; // ✅ 合并两个旋转

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
