
using UnityEngine;
using System.Collections;

public class Frost : HeroBase, IPlayerEffect
{
    private int availableInstantCooldowns = 0;

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

            Debug.LogWarning("Ice Javelin is still on cooldown!");
        }
    }

    protected override void UseAbility2()
    {
        if (ability2CooldownTimer <= 0f)
        {
            StartCoroutine(FrozenWall());
            ability2CooldownTimer = abilities.ability2.cooldown / (PowerSurgeActive ? 2f : 1f);
        }
        else
        {
            if (availableInstantCooldowns > 0)
            {
                ability2CooldownTimer = 0f;
                availableInstantCooldowns--;
            }

            Debug.LogWarning("Frozen Wall is still on cooldown!");
        }
    }

    protected override void UseUltimate()
    {
        if (ultimateCooldownTimer <= 0f)
        {
            StartCoroutine(AbsoluteZero());
            ultimateCooldownTimer = abilities.ultimate.cooldown / (PowerSurgeActive ? 2f : 1f);
        }
        else
        {
            if (availableInstantCooldowns > 0)
            {
                ultimateCooldownTimer = 0f;
                availableInstantCooldowns--;
            }

            Debug.LogWarning("Absolute Zero is still on cooldown!");
        }
    }

    private IEnumerator FrozenWall()
    {
        GameObject wall = Instantiate(abilities.ability2.projectilePrefab, transform.position + transform.forward * 2f, Quaternion.identity);

        yield return new WaitForSeconds(3f);

        Collider[] enemies = Physics.OverlapSphere(wall.transform.position, 3f);
        foreach (var enemy in enemies)
        {
            if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
            {
                enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ability2.damage);
                enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ability2.damage);
                enemy.GetComponent<StatusEffects>()?.ApplyStun(2f);
            }
        }

        Destroy(wall);
    }

    private IEnumerator AbsoluteZero()
    {
        GameObject zero = Instantiate(abilities.ultimate.projectilePrefab, transform.position + transform.forward * 2f, Quaternion.identity);

        Collider[] enemies = Physics.OverlapSphere(transform.position, 4f);
        foreach (var enemy in enemies)
        {
            if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
            {
                var status = enemy.GetComponent<StatusEffects>();
                status?.ApplyStun(1.5f);
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

        Destroy(zero, 10f);
    }

    #region Interface
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
        //
    }

    public void ResetAbilityCooldownTimer(int cooldown)
    {
        availableInstantCooldowns += cooldown;
    }
    #endregion
}
