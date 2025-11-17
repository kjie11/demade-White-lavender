using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DamagePostProcess : MonoBehaviour
{
    public Volume volume; // 拖入 Global Volume
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    private ChromaticAberration chromatic;

    public float effectDuration = 0.5f;
    private float timer = 0f;
    private bool isActive = false;

    void Start()
    {
        volume.profile.TryGet(out vignette);
        volume.profile.TryGet(out colorAdjustments);
         Debug.Log("✅ ColorAdjustments found");
        volume.profile.TryGet(out chromatic);
        Debug.Log("Profile is from asset: " + AssetDatabase.Contains(volume.profile));
        
    }

    void Update()
    {
        if (isActive)
        {
            timer += Time.deltaTime;
            float t = timer / effectDuration;

            // 恢复效果
            vignette.intensity.value = Mathf.Lerp(0.4f, 0f, t);
            colorAdjustments.saturation.value = Mathf.Lerp(-50f, 0f, t);
            chromatic.intensity.value = Mathf.Lerp(0.8f, 0f, t);

            if (t >= 1f)
                isActive = false;
        }
    }

    public void TriggerDamageEffect()
    {
        if (vignette == null) return;
        timer = 0f;
        isActive = true;

        // vignette.intensity.value = 0.4f;
        // vignette.color.value = Color.red;
        // colorAdjustments.saturation.value = -50f;
        // chromatic.intensity.value = 0.8f;
        colorAdjustments.postExposure.value = -5f;
colorAdjustments.colorFilter.value = Color.red;
colorAdjustments.saturation.value = -100f;

    }
}
