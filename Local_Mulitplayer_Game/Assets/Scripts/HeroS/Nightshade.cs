using System.Collections;
using UnityEngine;

public class Nightshade : HeroBase
{
    protected override void UseAbility1()
    {
        if (ability1CooldownTimer <= 0f)
        {
            ShootProjectile(abilities.ability1);
            ability1CooldownTimer = abilities.ability1.cooldown / (PowerSurgeActive ? 2f : 1f);
        }
        else
        {
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
            Debug.LogWarning("Grave Silence is still on cooldown!");
        }
    }

    private IEnumerator PhantomClone()
    {
        GameObject clone = Instantiate(abilities.ability2.projectilePrefab, transform.position + transform.forward * 1.5f, transform.rotation);

        yield return new WaitForSeconds(3f);

        if (clone != null) 
        {
            Collider[] enemies = Physics.OverlapSphere(clone.transform.position, 3f);
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
        Collider[] enemies = Physics.OverlapSphere(transform.position, 4f);
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
}
