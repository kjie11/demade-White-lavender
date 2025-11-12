using UnityEngine;
using System;
using System.Collections;

public class ExplosionCore : MonoBehaviour
{
    public event Action OnExplosionEnd; // 💥 爆炸结束事件
    public float explosionDuration = 0.8f; // 你的爆炸动画时间

    void Start()
    {
        // 这里可以播放你的动画 / 粒子效果
        StartCoroutine(ExplosionSequence());
    }

    private IEnumerator ExplosionSequence()
    {
        // 等待动画结束
        yield return new WaitForSeconds(explosionDuration);

        // 🔥 触发事件：通知UIManager可以生成金币了
        OnExplosionEnd?.Invoke();

        // 延迟一点再销毁自己（给玩家一点时间看到）
        Destroy(gameObject, 3f);
    }
}
