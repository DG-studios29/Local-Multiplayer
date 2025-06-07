using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Defines a contract between PickupSpawner && Arena Sampler classes
/// </summary>
public interface ISpawnable
{
    #region Interface

    void DoSpawn(List<Vector3> positions);

    #endregion
}
