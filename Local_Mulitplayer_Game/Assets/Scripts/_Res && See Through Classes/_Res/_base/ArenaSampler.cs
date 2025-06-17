using System.Collections.Generic;
using System.Collections;
using UnityEngine;
/// <summary>
/// This class samples, generate and validate positions within a bouding box
/// </summary>
public class ArenaSampler : MonoBehaviour
{
    #region Variables
    [Header("Sampler Variables")]
    [Space(10)]

    [SerializeField] private int numberOfPositions; // The number of positions to generate
    protected List<Vector3> generatedPositions = new List<Vector3> (); // stores generated positions

    [SerializeField] private BoxCollider boundingBox; // The bounding box (Box Collider) used to sample positions
    [SerializeField] private LayerMask filterMask;

    private ISpawnable spawnable; //ref to an object that implements ISpawnable interface

    private float sphereRadius = .5f;

    #endregion

    #region Built-In Methods
    private void OnEnable()
    {
        if (boundingBox!= null)
        {
            GenerateRandomizedPositions();

            spawnable = PickupSpawner.instance.GetComponent<ISpawnable>();

        }


    }


    private void Update()
    {
        if (boundingBox && generatedPositions.Count > 0)
        {
            DoWithSampledPositions();
        }
    }

    /// <summary>
    /// Visualize generated positions in the scene view
    /// </summary>
    private void OnDrawGizmos()
    {
        if (boundingBox && generatedPositions.Count > 0)
        {
            foreach(var pos in generatedPositions)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(pos, sphereRadius);
            }
        }
    }
    #endregion

    #region Custom Methods
    /// <summary>
    /// generate and validate positions
    /// </summary>
    private void GenerateRandomizedPositions()
    {
        //we clear the list on start
        generatedPositions.Clear ();

        for (int i = 0; i < numberOfPositions; i++)
        {
            Vector3 randomPos;

            var xAxis = Random.Range(boundingBox.bounds.min.x, boundingBox.bounds.max.x);
            var zAxis = Random.Range(boundingBox.bounds.min.z, boundingBox.bounds.max.z);

            randomPos = new Vector3(xAxis, 1.1f, zAxis);

            if (ValidatePositions(randomPos))
            {
                generatedPositions.Add(randomPos);
            }
            else
            {
                Debug.LogWarning("position not valid... discarding position now");
            }

            //Debug.Log(generatedPositions.Count);
        }
    }

    /// <summary>
    /// Validates generated positions
    /// <summary>
    private bool ValidatePositions(Vector3 pos)
    {
        //ensure all mesh colliders in the scene are set to convex for prper physics interactions!!
        return !Physics.CheckSphere(pos, sphereRadius, filterMask);
    }

    private void DoWithSampledPositions()
    {
        if (spawnable != null) spawnable.DoSpawn(generatedPositions);
    }
    
    #endregion
}
