using UnityEngine;

public class DestroySelf : MonoBehaviour
{
    private void OnEnable()
    {
        Destroy(gameObject, 1f);
    }
}
