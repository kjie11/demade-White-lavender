using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndSceneFadeController : MonoBehaviour
{
    [Header("UI")]
    public Image blackScreen;
    public TextMeshProUGUI endingText;

    [Header("Settings")]
    public float fadeSpeed = 1.2f;
    public float textDelay = 1f;
    public float endingDisplayTime = 4f;

    void Start()
    {
        // 初始化
        blackScreen.color = new Color(0, 0, 0, 1f);   // 全黑
        endingText.alpha = 0f;                        // 全透明

        StartCoroutine(PlayEndingSequence());
    }

    IEnumerator PlayEndingSequence()
    {
        yield return FadeInText();

        yield return new WaitForSeconds(endingDisplayTime);

        yield return FadeOutScreen();

        SceneManager.LoadScene("Start");   
    }

    IEnumerator FadeInText()
    {
        yield return new WaitForSeconds(textDelay);

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;

            // 黑幕淡出
            // blackScreen.color = new Color(0, 0, 0, 1f - t);

            // 字幕淡入
            endingText.alpha = t;

            yield return null;
        }
    }

    IEnumerator FadeOutScreen()
    {
        float t = 0f;

        // 淡出至黑屏
        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;

            blackScreen.color = new Color(0, 0, 0, t);
            endingText.alpha = 1f - t;

            yield return null;
        }
    }
}
