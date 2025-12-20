using UnityEngine;

public class AlwaysFlicker : MonoBehaviour
{
    [Header("Flicker Settings")]
    public float minAlpha = 0.2f;   // 最透明
    public float maxAlpha = 1.0f;   // 最不透明
    public float flickerSpeed = 1.5f; // 闪烁速度（越大越快）

    private Material mat;
    private Color originalColor;

    void Start()
    {
        Renderer renderer = GetComponentInChildren<Renderer>();
        mat = renderer.material;
        originalColor = mat.color;
    }

    void Update()
    {
        // t 在 0~1 之间来回变化，形成呼吸效果
        float t = (Mathf.Sin(Time.time * flickerSpeed) + 1f) * 0.5f;

        float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

        Color c = originalColor;
        c.a = alpha;
        mat.color = c;
    }
}
