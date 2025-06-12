using UnityEngine;
/// <summary>
/// heals the player
/// </summary>
public class HealingHerb : PickUpsBase
{
    #region Custom Variables

    [SerializeField] private float healthAmount;
    [SerializeField] private GameObject vFx;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private AudioSource audioSource;

    #endregion

    #region Overridden Methods

    protected override void ApplyEffect(GameObject player)
    {
        IPlayerEffect[] playerEffect = player.GetComponents<IPlayerEffect>();

        if (playerEffect.Length > 0)
        {
            foreach (var effect in playerEffect)
            {
                if (effect != null)
                {
                    effect.GiveHealth(healthAmount);
                    if (audioSource != null && audioClip != null) audioSource.PlayOneShot(audioClip);

                    if (vFx != null)
                    {
                        Instantiate(vFx, transform.position, Quaternion.identity);
                    }
                }
            }
        }

        Destroy(gameObject, 1f);
    }

    #endregion
}
