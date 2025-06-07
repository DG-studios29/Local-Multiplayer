using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class FadingObject : MonoBehaviour
{
    #region Custom Variables

    [Header("* Renderers (READ-ONLY) *"), Space(10f)]
    [SerializeField] private List<Renderer> renderers = new List<Renderer>();

    [HideInInspector] public float initialAlpha = 1f;
    [HideInInspector] public List<Material> materials = new List<Material>();

    private Coroutine fadeCoroutine;

    #endregion

    #region Built-In Methods

    private void Awake()
    {
        InitializeObject();
    }

    #endregion

    #region Custom Methods

    private void InitializeObject()
    {
        renderers.Clear();
        materials.Clear();

        switch (renderers.Count)
        {
            case (0):
                renderers.AddRange(GetComponentsInChildren<Renderer>());

                foreach (var renderer in renderers)
                {
                    materials.AddRange(renderer.materials);
                }

                break;
        }

    }
    #endregion
}
