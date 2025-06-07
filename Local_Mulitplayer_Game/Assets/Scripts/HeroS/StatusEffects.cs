
using System.Collections;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    private Coroutine burnRoutine, slowRoutine, stunRoutine, silenceRoutine, armorRoutine;

    public bool IsStunned { get; private set; }
    public bool IsSlowed { get; private set; }
    public bool IsSilenced { get; private set; }
    public bool HasArmorBuff { get; private set; }

    private float damageReductionMultiplier = 1f;

    public void ApplyBurn(float duration, int dps)
    {
        if (burnRoutine != null) StopCoroutine(burnRoutine);
        burnRoutine = StartCoroutine(Burn(duration, dps));
    }

    public void ApplySlow(float duration, float slowFactor)
    {
        if (slowRoutine != null) StopCoroutine(slowRoutine);
        slowRoutine = StartCoroutine(Slow(duration, slowFactor));
    }

    public void ApplyStun(float duration)
    {
        if (stunRoutine != null) StopCoroutine(stunRoutine);
        stunRoutine = StartCoroutine(Stun(duration));
    }

    public void ApplySilence(float duration)
    {
        if (silenceRoutine != null) StopCoroutine(silenceRoutine);
        silenceRoutine = StartCoroutine(Silence(duration));
    }

    public void ApplyArmorBuff(float duration, float reductionPercent)
    {
        if (armorRoutine != null) StopCoroutine(armorRoutine);
        armorRoutine = StartCoroutine(ArmorBuff(duration, reductionPercent));
    }

    private IEnumerator Burn(float duration, int dps)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            GetComponent<PlayerHealth>()?.TakeDamage(dps);
            GetComponent<EnemyAI>()?.TakeDamage(dps);
            yield return new WaitForSeconds(1f);
            elapsed += 1f;
        }
    }

    private IEnumerator Slow(float duration, float factor)
    {
        IsSlowed = true;
        // Reduce speed here using your movement system (e.g., player movement multiplier)
        yield return new WaitForSeconds(duration);
        IsSlowed = false;
    }

    private IEnumerator Stun(float duration)
    {
        IsStunned = true;
        // Disable movement or actions in your movement/AI logic
        yield return new WaitForSeconds(duration);
        IsStunned = false;
    }

    private IEnumerator Silence(float duration)
    {
        IsSilenced = true;
        // Prevent ability casting logic
        yield return new WaitForSeconds(duration);
        IsSilenced = false;
    }

    private IEnumerator ArmorBuff(float duration, float reductionPercent)
    {
        HasArmorBuff = true;
        damageReductionMultiplier = 1f - reductionPercent;
        yield return new WaitForSeconds(duration);
        HasArmorBuff = false;
        damageReductionMultiplier = 1f;
    }

    public int ModifyIncomingDamage(int baseDamage)
    {
        return Mathf.RoundToInt(baseDamage * damageReductionMultiplier);
    }
}
