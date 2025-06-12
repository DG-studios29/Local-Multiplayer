using UnityEngine;
using UnityEngine.Audio;
/// <summary>
/// grants the player a temporary shield.
/// </summary>
public class TempShield : PickUpsBase
{
    #region Custom Variables

    [SerializeField] private float shieldDuration;
    [SerializeField] private GameObject shieldGFX;
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
                    effect.ActivateShield(shieldDuration, shieldGFX);
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
