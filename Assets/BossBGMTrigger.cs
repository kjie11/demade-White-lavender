using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossBGMTrigger : MonoBehaviour
{
    public GameObject BigHealthBarcanvas; //big heath bar
    [Header("Fade Images")]
    public Image[] fadeImages;    //big heath bar image
    public float fadeDuration = 1f;

    private Coroutine fadeCoroutine;
    void Start()
    {
        BigHealthBarcanvas.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.EnterBossArea();
             BigHealthBarcanvas.SetActive(true);
             foreach (var img in fadeImages)
        {
            if (img == null) continue;
            Color c = img.color;
            c.a = 1f;
            img.color = c;
        }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.ExitBossArea();
           FadeOutImagesAndDisableCanvas();
        }
            
    }

    //ai
    void FadeOutImagesAndDisableCanvas()
    {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutImages());
    }

    IEnumerator FadeOutImages()
    {
        float time = 0f;

        Color[] startColors = new Color[fadeImages.Length];
        for (int i = 0; i < fadeImages.Length; i++)
            startColors[i] = fadeImages[i].color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float t = time / fadeDuration;

            for (int i = 0; i < fadeImages.Length; i++)
            {
                if (fadeImages[i] == null) continue;

                Color c = startColors[i];
                c.a = Mathf.Lerp(startColors[i].a, 0f, t);
                fadeImages[i].color = c;
            }

            yield return null;
        }

        // 确保完全透明
        foreach (var img in fadeImages)
        {
            if (img == null) continue;
            Color c = img.color;
            c.a = 0f;
            img.color = c;
        }

        BigHealthBarcanvas.SetActive(false);
    }
}
