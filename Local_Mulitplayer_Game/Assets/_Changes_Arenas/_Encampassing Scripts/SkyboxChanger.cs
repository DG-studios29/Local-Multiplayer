using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    #region Variable

    [SerializeField] private Material skyboxMaterial;

    #endregion

    #region Built-In Method

    private void OnEnable()
    {
        if (skyboxMaterial == null) return;
        RenderSettings.skybox = skyboxMaterial;
    }

    #endregion
}
