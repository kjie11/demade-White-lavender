using UnityEngine;

public class BossBGMTrigger : MonoBehaviour
{
   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.Instance.EnterBossArea();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            AudioManager.Instance.ExitBossArea();
    }
}
