using UnityEngine;

/// <summary>
/// grants temporary speed boost
/// </summary>
public class SpeedBoost : PickUpsBase
{
    #region Custom Variables

    [Header("Speed Boost Properties"), Space(10f)]

    [SerializeField] private float duration = 5f;
    [SerializeField] private float speedBoost = 10f;
    [SerializeField] private GameObject pickUpEffect;
    [SerializeField] private GameObject trailEffect;
    #endregion

    #region Overridden Methods

    protected override void ApplyEffect(GameObject player)
    {
        if(pickUpEffect)
        {
            GameObject pEffect = Instantiate(pickUpEffect, transform.position, Quaternion.identity);
            Destroy(pEffect, 1f);
        }

        IPlayerEffect[] playerEffect = player.GetComponents<IPlayerEffect>();
        if (playerEffect.Length > 0)
        {
            foreach (var effect in playerEffect)
            {
                effect.ActivateSpeedBoost(duration, speedBoost, trailEffect);
            }
        }

        Destroy(gameObject);
    }

    #endregion
}
