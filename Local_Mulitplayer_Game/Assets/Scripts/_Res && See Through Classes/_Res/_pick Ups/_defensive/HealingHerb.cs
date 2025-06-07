using UnityEngine;
/// <summary>
/// heals the player
/// </summary>
public class HealingHerb : PickUpsBase
{
    #region Custom Variables

    [SerializeField] private float healthAmount;
    [SerializeField] private GameObject pickUpEffect;

    #endregion

    #region Overridden Methods

    protected override void ApplyEffect(GameObject player)
    {
        IPlayerEffect []playerEffect = player.GetComponents<IPlayerEffect>();
        if(playerEffect.Length>0)
        {
            foreach (var effect in playerEffect)
            {
                effect.GiveHealth(healthAmount);
            }
        }

        Destroy(gameObject);
    }

    #endregion
}
