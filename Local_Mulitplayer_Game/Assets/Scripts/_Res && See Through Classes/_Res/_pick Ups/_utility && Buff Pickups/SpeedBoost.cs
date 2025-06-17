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
    [SerializeField] private GameObject vFx;
    [SerializeField] private GameObject sfx;
    [SerializeField] private AudioClip sfxClip;

    [SerializeField] private GameObject trailEffect;
    #endregion

    #region Overridden Methods

    protected override void ApplyEffect(GameObject player)
    {
        IPlayerEffect[] playerEffect = player.GetComponents<IPlayerEffect>();

        if (playerEffect.Length > 0)
        {
            foreach (var effect in playerEffect)
            {
                effect.ActivateSpeedBoost(duration, speedBoost, trailEffect);

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

        Destroy(gameObject);
    }

    #endregion
}

