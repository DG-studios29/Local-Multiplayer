
using UnityEngine;
using System.Collections;

public class Frost : HeroBase
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
            Debug.LogWarning("Absolute Zero is still on cooldown!");
        }
    }

    private IEnumerator FrozenWall()
    {
        GameObject wall = Instantiate(abilities.ability2.projectilePrefab, transform.position + transform.forward * 2f, Quaternion.identity);

        yield return new WaitForSeconds(3f);

        if (wall != null)
        {
            Collider[] enemies = Physics.OverlapSphere(wall.transform.position, 3f);
            foreach (var enemy in enemies)
            {
                if (enemy == null || enemy.gameObject == null) continue;
                if ((enemy.CompareTag("Enemy")|| enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
                {
                    enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ability2.damage, gameObject);
                    enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ability2.damage, gameObject);
                    enemy.GetComponent<StatusEffects>()?.ApplyStun(2f);
                }
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
            if (enemy == null || enemy.gameObject == null) continue;
            if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
            {
                var status = enemy.GetComponent<StatusEffects>();
                status?.ApplyStun(1.5f);
            }
        }

        yield return new WaitForSeconds(1.5f);

        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.gameObject == null) continue;
            if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
            {
                enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ultimate.damage, gameObject);
                enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ultimate.damage, gameObject);
            }
        }

        Destroy(zero, 10f);
    }
}
