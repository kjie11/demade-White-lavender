using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main == null) return;

        Vector3 camForward=Camera.main.transform.forward;
        camForward.y=0f;
        if (camForward.sqrMagnitude < 0.001f)
        {
            return;
        }
        transform.forward=camForward.normalized;
        // transform.LookAt(
        //     transform.position + Camera.main.transform.forward,
        //     Camera.main.transform.up
        // );
    }
}
