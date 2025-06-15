using UnityEngine;

/// <summary>
/// 
/// </summary>
public class CoolDownReset : PickUpsBase
{
    #region Custom Variables

    private int coolDown = 1;
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
                    effect.ResetAbilityCooldownTimer(coolDown);
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
