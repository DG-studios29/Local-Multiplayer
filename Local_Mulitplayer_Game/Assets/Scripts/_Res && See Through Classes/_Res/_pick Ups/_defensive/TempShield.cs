using UnityEngine;
/// <summary>
/// grants the player a temporary shield.
/// </summary>
public class TempShield : PickUpsBase
{
    #region Custom Variables

    [SerializeField] private float shieldDuration;
    [SerializeField] private GameObject pickUpEffect;
    [SerializeField] private GameObject shieldBubble;
    #endregion

    #region Overridden Methods

    protected override void ApplyEffect(GameObject player)
    {
        IPlayerEffect[] playerEffect = player.GetComponents<IPlayerEffect>();
        if (playerEffect.Length > 0)
        {
            foreach (var effect in playerEffect)
            {
                effect.ActivateShield(shieldDuration, shieldBubble);
            }
        }

        Destroy(gameObject);
    }

    #endregion
}
