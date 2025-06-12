using UnityEngine;

public class VFxSelfDestruct : MonoBehaviour
{
    #region Built-In Method

    private void OnEnable()
    {
        Destroy(gameObject, 1.5f);
    }

    #endregion
}

