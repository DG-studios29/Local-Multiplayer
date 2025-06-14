using System.Collections;
using UnityEngine;

public class Nightshade : HeroBase, IPlayerEffect
{
    private int availableInstantCooldowns;

    protected override void UseAbility1()
    {
        if (ability1CooldownTimer <= 0f)
        {
            ShootProjectile(abilities.ability1);
            ability1CooldownTimer = abilities.ability1.cooldown / (PowerSurgeActive ? 2f : 1f);
        }
        else
        {
            if (availableInstantCooldowns > 0)
            {
                ability1CooldownTimer = 0f;
                availableInstantCooldowns--;
            }

            Debug.LogWarning("Shadow Bolt is still on cooldown!");
        }
    }

    protected override void UseAbility2()
    {
        if (ability2CooldownTimer <= 0f)
        {
            StartCoroutine(PhantomClone());
            ability2CooldownTimer = abilities.ability2.cooldown / (PowerSurgeActive ? 2f : 1f);
        }
        else
        {
            if (availableInstantCooldowns > 0)
            {
                ability2CooldownTimer = 0f;
                availableInstantCooldowns--;

            }
            Debug.LogWarning("Phantom Clone is still on cooldown!");
        }
    }

    protected override void UseUltimate()
    {
        if (ultimateCooldownTimer <= 0f)
        {
            StartCoroutine(GraveSilence());
            ultimateCooldownTimer = abilities.ultimate.cooldown / (PowerSurgeActive ? 2f : 1f);
        }
        else
        {
            if (availableInstantCooldowns > 0)
            {
                ultimateCooldownTimer = 0f;
                availableInstantCooldowns--;
            }

            Debug.LogWarning("Grave Silence is still on cooldown!");
        }
    }

    private IEnumerator PhantomClone()
    {
        GameObject clone = Instantiate(abilities.ability2.projectilePrefab, transform.position + transform.forward * 1.5f, transform.rotation);

        yield return new WaitForSeconds(3f);

        if (clone != null) 
        {
            Collider[] enemies = Physics.OverlapSphere(clone.transform.position, 5f);
            foreach (var enemy in enemies)
            {
                if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
                {
                    enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ability2.damage);
                    enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ability2.damage);
                    enemy.GetComponent<StatusEffects>()?.ApplySilence(2f);
                }
            }

            Destroy(clone); 
        }
    }


    private IEnumerator GraveSilence()
    {
        if (abilities.ultimate.projectilePrefab != null)
        {
            GameObject effect = Instantiate(abilities.ultimate.projectilePrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            Destroy(effect, 3f);
        }

        Collider[] enemies = Physics.OverlapSphere(transform.position, 10f);
        foreach (var enemy in enemies)
        {
            if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
            {
                var status = enemy.GetComponent<StatusEffects>();
                status?.ApplySilence(2f);
                status?.ApplySlow(2f, 0.5f);
                status?.ApplyBlind(2f);
            }
        }

        yield return new WaitForSeconds(1.5f);

        foreach (var enemy in enemies)
        {
            if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
            {
                enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ultimate.damage);
                enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ultimate.damage);
            }
        }
    }

    #region

    public void ActivateShield(float duration, GameObject shield)
    {

    }

    public void ActivateSpeedBoost(float duration, float speedMultiplier, GameObject trailEffect)
    {

    }

    public void GiveHealth(float health)
    {

    }

    public void RefillAbilityBar()
    {

    }

    public void ResetAbilityCooldownTimer(int cooldown)
    {
        availableInstantCooldowns += cooldown;
    }

    #endregion
}
