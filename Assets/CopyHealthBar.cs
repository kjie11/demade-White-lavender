using UnityEngine;
using UnityEngine.UI;

public class CopyHealthBar : MonoBehaviour
{
     [Header("UI References")]
    public Slider slider;      
    public Image fillImage;   
    public GameObject canvas;

    void Update()
    {
        if (slider != null && fillImage != null)
        {
            fillImage.fillAmount = slider.value/100; //jump enemy max health is 100
        }
        if (slider == null)
        {
            canvas.SetActive(false);
        }
    }
}
