using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource audioSource;
    public AudioClip bgm;
    public AudioClip bossBGM; 
    void Awake()
    {
        // 单例模式：场景中只保留一个 AudioManager
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 切换场景不销毁
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 确保存在 AudioSource 组件
        audioSource = gameObject.AddComponent<AudioSource>();
    }
     void Start()
    {
        AudioManager.Instance.PlayBGM(bgm, 0.1f);
    }

    /// <summary>
    /// 播放一次性音效（例如：攻击、跳跃、点击）
    /// </summary>
    /// 
    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Tried to play a null AudioClip!");
            return;
        }

        audioSource.PlayOneShot(clip);
    }

    //ai
    public void PlayBGM(AudioClip clip, float fadeTime = 1f)
{
    StartCoroutine(FadeToNewBGM(clip, fadeTime));
}
public void EnterBossArea()
{
    PlayBGM(bossBGM, 0.02f);
}

public void ExitBossArea()
{
    PlayBGM(bgm, 0.3f);
}
//ai
private IEnumerator FadeToNewBGM(AudioClip newClip, float duration)
{
    if (audioSource.clip == newClip)
        yield break;  // 不重复播放同个音乐

    float startVolume = audioSource.volume;

    // 1️⃣ Fade Out
    float t = 0f;
    while (t < duration)
    {
        t += Time.deltaTime;
        audioSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
        yield return null;
    }

    // 2️⃣ 切换音乐
    audioSource.clip = newClip;
    audioSource.loop = true;
    audioSource.Play();

    // 3️⃣ Fade In
    t = 0f;
    while (t < duration)
    {
        t += Time.deltaTime;
        audioSource.volume = Mathf.Lerp(0f, startVolume, t / duration);
        yield return null;
    }
}

    /// <summary>
    /// 播放循环音效（例如：背景音乐）
    /// </summary>
    public void PlayLoop(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Tried to play a null looping clip!");
            return;
        }

        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    /// <summary>
    /// 停止当前播放的音效
    /// </summary>
    public void Stop()
    {
        audioSource.Stop();
        audioSource.loop = false;
    }
}
