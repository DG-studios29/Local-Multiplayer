using UnityEngine;
/// <summary>
/// Base class for all pickups to inherit from
/// </summary>
public abstract class PickUpsBase : MonoBehaviour
{
    #region Built- In Methods

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            ApplyEffect(other.gameObject);
            PickupSpawner.instance.currentlySpawnedPickUps--;
        }
    }

    #endregion

    #region Custom Methods

    protected abstract void ApplyEffect(GameObject player);

    #endregion
}
