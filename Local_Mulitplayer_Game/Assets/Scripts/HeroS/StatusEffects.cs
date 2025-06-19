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

    [Header("Visual Effect Prefabs")]
    public GameObject burnEffectPrefab;
    public GameObject freezeEffectPrefab;
    public GameObject slowEffectPrefab;
    public GameObject silenceEffectPrefab;
    public GameObject armorEffectPrefab;
    public GameObject blindEffectPrefab;

    private float originalMoveSpeed = -1f;

    public void ApplyBurn(float duration, int dps)
    {
        if (burnRoutine != null) StopCoroutine(burnRoutine);
        burnRoutine = StartCoroutine(Burn(duration, dps));
        SpawnEffect(burnEffectPrefab, duration);
    }

    public void ApplySlow(float duration, float slowFactor)
    {
        if (slowRoutine != null) StopCoroutine(slowRoutine);
        slowRoutine = StartCoroutine(Slow(duration, slowFactor));
        SpawnEffect(slowEffectPrefab, duration);
    }

    private bool isStunned = false;

    public void ApplyStun(float duration)
    {
        if (isStunned) return;
        isStunned = true;

        SpawnEffect(freezeEffectPrefab, duration);
        StartCoroutine(Stun(duration));
    }

    private IEnumerator Stun(float duration)
    {
        IsStunned = true;

        var controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.enabled = false;

        var ai = GetComponent<EnemyAI>();
        if (ai != null)
            ai.enabled = false;

        yield return new WaitForSeconds(duration);

        if (controller != null)
            controller.enabled = true;

        if (ai != null)
            ai.enabled = true;

        IsStunned = false;
        isStunned = false;
    }

    public void ApplySilence(float duration)
    {
        if (silenceRoutine != null) StopCoroutine(silenceRoutine);
        silenceRoutine = StartCoroutine(Silence(duration));
        SpawnEffect(silenceEffectPrefab, duration);
    }

    public void ApplyArmorBuff(float duration, float reductionPercent)
    {
        if (armorRoutine != null) StopCoroutine(armorRoutine);
        armorRoutine = StartCoroutine(ArmorBuff(duration, reductionPercent));
        SpawnEffect(armorEffectPrefab, duration);
    }

    public void ApplyBlind(float duration)
    {
        if (blindRoutine != null) StopCoroutine(blindRoutine);
        blindRoutine = StartCoroutine(Blind(duration));
        SpawnEffect(blindEffectPrefab, duration);
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

        var move = GetComponent<PlayerController>();
        if (move != null)
        {
            if (originalMoveSpeed < 0f)
                originalMoveSpeed = move.moveSpeed;

            move.moveSpeed *= factor;
        }

        yield return new WaitForSeconds(duration);

        if (move != null && originalMoveSpeed > 0f)
            move.moveSpeed = originalMoveSpeed;

        IsSlowed = false;
    }

    private IEnumerator Silence(float duration)
    {
        IsSilenced = true;

        var hero = GetComponent<HeroBase>();
        if (hero != null)
        {
            hero.canCastAbilities = false;
        }

        yield return new WaitForSeconds(duration);

        if (hero != null)
        {
            hero.canCastAbilities = true;
        }

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

    private GameObject SpawnEffect(GameObject effectPrefab, float duration)
    {
        if (effectPrefab == null) return null;

        GameObject vfx = Instantiate(effectPrefab, transform.position, Quaternion.identity, transform);
        Destroy(vfx, duration);
        return vfx;
    }
}
