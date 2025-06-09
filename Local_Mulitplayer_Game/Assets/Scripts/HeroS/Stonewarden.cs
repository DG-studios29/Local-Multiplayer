
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Stonewarden : HeroBase
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
            Debug.LogWarning("Stone Fist is still on cooldown!");
        }
    }

    protected override void UseAbility2()
    {
        if (ability2CooldownTimer <= 0f)
        {
            StartCoroutine(Fortify());
            ability2CooldownTimer = abilities.ability2.cooldown / (PowerSurgeActive ? 2f : 1f);
        }
        else
        {
            Debug.LogWarning("Fortify is still on cooldown!");
        }
    }

    protected override void UseUltimate()
    {
        if (ultimateCooldownTimer <= 0f)
        {
            StartCoroutine(SeismicRift());
            ultimateCooldownTimer = abilities.ultimate.cooldown / (PowerSurgeActive ? 2f : 1f);
        }
        else
        {
            Debug.LogWarning("Seismic Rift is still on cooldown!");
        }
    }

    private IEnumerator Fortify()
    {
        GetComponent<StatusEffects>()?.ApplyArmorBuff(5f, 0.5f); // Reduce 50% damage for 5 seconds
        GameObject rock = Instantiate(abilities.ability2.projectilePrefab, transform.position + transform.forward * 2f, Quaternion.identity);

        Destroy(rock, 5f);
        yield return null;

        
    }

    private IEnumerator SeismicRift()
    {
        int segments = 6;
        float delayBetweenSegments = 0.2f;
        float range = 7f;
        float segmentSpacing = range / segments;
        Vector3 direction = transform.forward;

        for (int i = 1; i <= segments; i++)
        {
            Vector3 segmentPosition = transform.position + direction * (i * segmentSpacing);
            GameObject riftSegment = Instantiate(abilities.ultimate.projectilePrefab, segmentPosition, Quaternion.identity);

            Collider[] enemies = Physics.OverlapBox(segmentPosition, new Vector3(2f, 2f, 2f));
            foreach (var enemy in enemies)
            {
                if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
                {
                    enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ultimate.damage);
                    enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ultimate.damage);
                    enemy.GetComponent<StatusEffects>()?.ApplyStun(1f);
                }
            }

            yield return new WaitForSeconds(delayBetweenSegments);
        }
    }
}
