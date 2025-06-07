
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stonewarden : HeroBase
{
    protected override void UseAbility1()
    {
        if (ability1CooldownTimer <= 0f)
        {
            // Stone Fist - creates rubble on impact
            ShootProjectile(abilities.ability1);
            ability1CooldownTimer = abilities.ability1.cooldown;
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
            Debug.Log("Stonewarden activates Fortify!");
            StartCoroutine(Fortify());
            ability2CooldownTimer = abilities.ability2.cooldown;
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
            Debug.Log("Stonewarden unleashes Seismic Rift!");
            StartCoroutine(SeismicRift());
            ultimateCooldownTimer = abilities.ultimate.cooldown;
        }
        else
        {
            Debug.LogWarning("Seismic Rift is still on cooldown!");
        }
    }

    private IEnumerator Fortify()
    {
        float shieldDuration = 5f;

        var status = GetComponent<StatusEffects>();
        if (status != null)
        {
            status.ApplyArmorBuff(shieldDuration, 0.5f); // 50% damage reduction
        }

        // Reflect projectiles manually if needed in your combat system
        yield return new WaitForSeconds(shieldDuration);
    }

    private IEnumerator SeismicRift()
    {
        float riftRange = 10f;
        float width = 2f;
        int segments = 5;
        float delayBetweenSegments = 0.2f;

        Vector3 start = transform.position + transform.forward * 2f;

        for (int i = 0; i < segments; i++)
        {
            Vector3 segmentPos = start + transform.forward * i * (riftRange / segments);
            GameObject riftEffect = Instantiate(abilities.ultimate.projectilePrefab, segmentPos, Quaternion.identity);
            Destroy(riftEffect, 2f);

            Collider[] hit = Physics.OverlapBox(segmentPos, new Vector3(width, 1, width));
            foreach (var enemy in hit)
            {
                if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
                {
                    enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ultimate.damage);
                    enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ultimate.damage);

                    var status = enemy.GetComponent<StatusEffects>();
                    if (status != null)
                        status.ApplyStun(1f);

                    Rigidbody rb = enemy.GetComponent<Rigidbody>();
                    if (rb != null)
                        rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
                }
            }

            yield return new WaitForSeconds(delayBetweenSegments);
        }
    }
}
