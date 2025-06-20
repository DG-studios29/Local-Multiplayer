using UnityEngine;

public class Billboard : MonoBehaviour
{
    void LateUpdate()
    {
        if (Camera.main == null) return;

        Vector3 lookDirection = transform.position + Camera.main.transform.rotation * Vector3.forward;
        Vector3 upDirection = Camera.main.transform.rotation * Vector3.up;

        transform.LookAt(lookDirection, upDirection);
    }
}
