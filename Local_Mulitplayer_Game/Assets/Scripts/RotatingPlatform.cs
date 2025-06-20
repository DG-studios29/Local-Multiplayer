using UnityEngine;

public class RotatingPlatform : MonoBehaviour
{
    [Header("Rotation Speed (degrees per second)")]
    public Vector3 rotationSpeed = new Vector3(0f, 30f, 0f);

    void Update()
    {
        
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
