using UnityEngine;

/// <summary>
/// grants instant energy restoration for abilities
/// </summary>
public class EnergyCrystal : PickUpsBase
{
    #region Custom Variables
    [SerializeField] private GameObject vFx;
    [SerializeField] private GameObject sfx;
    [SerializeField] private AudioClip sfxClip;

    #endregion

    #region Overridden Methods

    protected override void ApplyEffect(GameObject player)
    {
        IPlayerEffect[] playerEffect = player.GetComponentsInChildren<IPlayerEffect>();

        if (playerEffect.Length > 0)
        {
            foreach (var effect in playerEffect)
            {
                if (effect != null)
                {
                    effect.RestoreOrbs();

                    if (vFx != null)
                    {
                        Instantiate(vFx, transform.position, Quaternion.identity);
                    }

                    if (sfx != null && sfxClip != null)
                    {
                        GameObject sfxObj = Instantiate(sfx, transform.position, Quaternion.identity);
                        sfxObj.GetComponent<SFxSoundPlayer>().DoAwayWithMe(sfxClip);
                    }
                }
            }
        }

        Destroy(gameObject);
    }

    #endregion
}
