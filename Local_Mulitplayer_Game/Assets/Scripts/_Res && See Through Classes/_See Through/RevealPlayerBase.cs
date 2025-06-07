using UnityEngine;
using System.Collections.Generic;

public class RevealPlayerBase : MonoBehaviour
{
    #region Custom Variables

    public static RevealPlayerBase instance;

    [Header("* Fade Properties *"), Space(10)]
    [SerializeField, Tooltip("- Main Camera")] private Transform cam;
    [SerializeField, Tooltip("- The Layer/s The Ray Can Interact With")] private LayerMask filterMask;
    [SerializeField, Range(0.1f, 1f), Tooltip("- The amount to fade the alpha to")] 
    private float fadeAmount;

    [Header("* Read-Only Properties (DO NOT MODIFY) *"), Space(10)]
    [SerializeField, Tooltip("- A List Of All Objects Currently Blocking The Camera's View")] 
    private List<FadingObject> objectsBlockingView = new List<FadingObject>();
    [Tooltip("- A List Of Player Transforms")] 
    public List<Transform> players = new List<Transform>();
    private HashSet<FadingObject> objectsCurrentlyBeingHit = new HashSet<FadingObject>();
    private RaycastHit[] _hits = new RaycastHit[5];//max number of objects that can be hit within a single update frame

    #endregion

    #region Built-In Methods
    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        if (!cam) cam = Camera.main.transform;
        
    }
    //private void OnValidate()
    //{
    //    if(!cam) cam = Camera.main.transform;
    //}

    private void Update()
    {
        CheckForBlockingObjects();
    }

    #endregion

    #region Custom Methods
    private void CheckForBlockingObjects()
    {
        if (cam && players.Count > 0)
        {
            objectsCurrentlyBeingHit.Clear();

            foreach(var player in players)
            {
                Vector3 rayDirection = player.position - cam.position; //finds the direction from the camera to each player
                float distance = rayDirection.magnitude; //utilizes the rayDirection's length to determine the distance

                int hits = Physics.RaycastNonAlloc(cam.position, rayDirection, _hits, distance, filterMask);
                Debug.DrawRay(cam.position, rayDirection * distance, Color.yellow);

                if(hits > 0)
                {
                    for(int i = 0; i < hits; i++)
                    {
                        FadingObject fadingObject = GetFadingObject(_hits[i]);

                        if(fadingObject != null)
                        {
                            objectsCurrentlyBeingHit.Add(fadingObject);

                            if(!objectsBlockingView.Contains(fadingObject))
                            {
                                objectsBlockingView.Add(fadingObject);
                                FadeOut(fadingObject);
                            }
                        }
                    }
                }
            }

            RemoveFadingObjectsNoLongerBeingHit();
        }
    }

    private void RemoveFadingObjectsNoLongerBeingHit()
    {
        //we set i to the size of the list, then...
        //check if i >= 0 && if true...
        //we keep decrementing i by 1 
        for(int i = objectsBlockingView.Count -1; i>= 0; --i)
        {
            if(!objectsCurrentlyBeingHit.Contains(objectsBlockingView[i]))
            {
                FadeIn(objectsBlockingView[i]);
                objectsBlockingView.Remove(objectsBlockingView[i]);
            }
        }
    }

    private FadingObject GetFadingObject(RaycastHit hit)
    {
        return hit.collider != null? hit.collider.GetComponent<FadingObject>() : null;
    }


    private void FadeOut(FadingObject fadingObject)
    {
        if(fadingObject.materials.Count > 0)
        {
            foreach (var material in fadingObject.materials)
            {
                //we cast these as integers because blendmodes are in all in the format of integers
                material.SetFloat("_SrcBlend", (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
                material.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                material.SetFloat("_ZWrite", 0);
                material.SetFloat("_Surface", 1);

                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

                material.SetShaderPassEnabled("MOTIONVECTORS", false);
                material.SetShaderPassEnabled("DepthOnly", false);
                material.SetShaderPassEnabled("SHADOWCASTER", false);

                material.SetOverrideTag("RenderType", "Transparent");

                //keywords
                material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }


            foreach (var material in fadingObject.materials)
            {
                if (material.HasProperty("_BaseColor"))
                {
                    material.color = new Color
                    (
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        fadeAmount
                    );
                }
            }
        }
    }

    private void FadeIn(FadingObject fadingObject)
    {
        if (fadingObject.materials.Count > 0)
        {
            foreach (var material in fadingObject.materials)
            {
                if (material.HasProperty("_BaseColor"))
                {
                    material.color = new Color
                    (
                        material.color.r,
                        material.color.g,
                        material.color.b,
                        fadingObject.initialAlpha
                    );
                }
            }

            foreach (var material in fadingObject.materials)
            {
                //we cast these as integers because blendmodes are in all in the format of integers
                material.SetFloat("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One); //default for opaque surface type
                material.SetFloat("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero); //default for opaque surface type
                material.SetFloat("_ZWrite", 1);
                material.SetFloat("_Surface", 0);

                material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;

                material.SetShaderPassEnabled("MOTIONVECTORS", false);
                material.SetShaderPassEnabled("DepthOnly", true);
                material.SetShaderPassEnabled("SHADOWCASTER", true);

                material.SetOverrideTag("RenderType", "Opaque");

                //keywords
                material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
            }
        }
    }

    #endregion
}
