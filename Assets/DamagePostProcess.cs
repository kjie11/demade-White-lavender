using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DamagePostProcess : MonoBehaviour
{
    // public Volume volume; // 拖入 Global Volume
    public GameObject volume;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    private ChromaticAberration chromatic;

    public float effectDuration = 0.5f;
    private float timer = 0f;
    private bool isActive = false;

    void Start()
    {
        volume.SetActive(false);
        
    }

    void Update()
    {
        
    }

    public void TriggerDamageEffect()
    {
        
        volume.SetActive(true);
        isActive=true;
        timer=0f;

    }
    
    public void recover()
    {
        isActive = false;

        volume.SetActive(false);
       
    }
}
