using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private AudioSource audioSource;

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

    /// <summary>
    /// 播放一次性音效（例如：攻击、跳跃、点击）
    /// </summary>
    public void PlaySound(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Tried to play a null AudioClip!");
            return;
        }

        audioSource.PlayOneShot(clip);
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
