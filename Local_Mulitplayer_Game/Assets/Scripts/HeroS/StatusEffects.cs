
using System.Collections;
using UnityEngine;

public class StatusEffects : MonoBehaviour
{
    private Coroutine burnRoutine, slowRoutine, stunRoutine, silenceRoutine, armorRoutine, blindRoutine;

    public bool IsStunned { get; private set; }
    public bool IsSlowed { get; private set; }
    public bool IsSilenced { get; private set; }
    public bool HasArmorBuff { get; private set; }

    public bool IsBlinded { get; private set; }

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

    private bool isStunned = false;

    public void ApplyStun(float duration)
    {
        if (isStunned) return;

        isStunned = true;

        // Disable movement or input
        var controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.enabled = false;

        //  spawn freeze effect
        // SpawnEffect(freezeEffectPrefab);

        StartCoroutine(Stun(duration));
    }

    private IEnumerator Stun(float duration)
    {
        IsStunned = true;

        // PLAYER: disable movement
        var controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.enabled = false;

        // ENEMY: disable AI
        var ai = GetComponent<EnemyAI>();
        if (ai != null)
            ai.enabled = false;

        yield return new WaitForSeconds(duration);

        // PLAYER: enable movement
        if (controller != null)
            controller.enabled = true;

        // ENEMY: re-enable AI
        if (ai != null)
            ai.enabled = true;

        IsStunned = false;
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

    public void ApplyBlind(float duration)
    {
        if (blindRoutine != null) StopCoroutine(blindRoutine);
        blindRoutine = StartCoroutine(Blind(duration));
    }

    private IEnumerator Burn(float duration, int dps)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            GetComponent<PlayerHealth>()?.TakeDamage(dps, gameObject);
            GetComponent<EnemyAI>()?.TakeDamage(dps, gameObject);
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

    private IEnumerator Blind(float duration)
    {
        IsBlinded = true;
        // OPTIONAL: trigger screen UI fade or disable aiming
        yield return new WaitForSeconds(duration);
        IsBlinded = false;
    }


    public int ModifyIncomingDamage(int baseDamage)
    {
        return Mathf.RoundToInt(baseDamage * damageReductionMultiplier);
    }
}
