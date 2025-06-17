using UnityEngine;

public class SFxSoundPlayer : MonoBehaviour
{
    #region Custom Vars
    [HideInInspector] public AudioClip sfxClip;
    public AudioSource sfxSource;
    [HideInInspector] private float waitTime = 1.5f; 
    #endregion

    #region Custom Methods

    public void DoAwayWithMe(AudioClip sfxClip)
    {
        sfxSource.PlayOneShot(sfxClip);
        Destroy(gameObject, waitTime);
    }

    #endregion
}
