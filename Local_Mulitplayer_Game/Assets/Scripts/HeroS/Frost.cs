
using UnityEngine;
using System.Collections;

public class Frost : HeroBase
{
    protected override void UseAbility1()
    {
        if (ability1CooldownTimer <= 0f)
        {
            // Ice Javelin - slows and shatters frozen targets
            ShootProjectile(abilities.ability1);
            ability1CooldownTimer = abilities.ability1.cooldown;
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
            Debug.Log("Frost conjures a Frozen Wall!");
            StartCoroutine(FrozenWall());
            ability2CooldownTimer = abilities.ability2.cooldown;
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
            Debug.Log("Frost activates Absolute Zero!");
            StartCoroutine(AbsoluteZero());
            ultimateCooldownTimer = abilities.ultimate.cooldown;
        }
        else
        {
            Debug.LogWarning("Absolute Zero is still on cooldown!");
        }
    }

    private IEnumerator FrozenWall()
    {
        Vector3 wallPosition = transform.position + transform.forward * 2f;
        GameObject wall = Instantiate(abilities.ability2.projectilePrefab, wallPosition, Quaternion.identity);

        yield return new WaitForSeconds(3f);

        Collider[] enemies = Physics.OverlapSphere(wall.transform.position, 3f);
        foreach (var enemy in enemies)
        {
            if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
            {
                var status = enemy.GetComponent<StatusEffects>();
                if (status != null)
                {
                    status.ApplyStun(1f); // explosion freezes briefly
                }

                enemy.GetComponent<PlayerHealth>()?.TakeDamage(abilities.ability2.damage);
                enemy.GetComponent<EnemyAI>()?.TakeDamage(abilities.ability2.damage);
            }
        }

        Destroy(wall);
    }

    private IEnumerator AbsoluteZero()
    {
        float freezeRadius = 5f;
        float freezeTime = 1.5f;
        float explosionDelay = 1.5f;

        Collider[] frozenTargets = Physics.OverlapSphere(transform.position, freezeRadius);
        foreach (var enemy in frozenTargets)
        {
            if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
            {
                var status = enemy.GetComponent<StatusEffects>();
                if (status != null)
                    status.ApplyStun(freezeTime);
            }
        }

        yield return new WaitForSeconds(explosionDelay);

        foreach (var enemy in frozenTargets)
        {
            if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
            {
                enemy.GetComponent<PlayerHealth>()?.TakeDamage(abilities.ultimate.damage);
                enemy.GetComponent<EnemyAI>()?.TakeDamage(abilities.ultimate.damage);
            }
        }
    }
}
