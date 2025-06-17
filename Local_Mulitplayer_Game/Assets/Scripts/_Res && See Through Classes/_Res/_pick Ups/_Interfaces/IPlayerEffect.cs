using UnityEngine;
/// <summary>
/// Defines a contract between pickups and PlayerPickupManager classes
/// </summary>
public interface IPlayerEffect
{
    #region Interface
    void ActivateSpeedBoost(float duration, float speedMultiplier, GameObject trailEffect);

    void ActivateShield(float duration, GameObject shield);
    void GiveHealth(float health);

    void ResetAbilityCooldownTimer(int cooldown);

    void RestoreOrbs();
    #endregion
}
