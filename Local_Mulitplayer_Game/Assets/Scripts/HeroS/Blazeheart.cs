
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blazeheart : HeroBase
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
            Debug.LogWarning("Molten Shot is still on cooldown!");
        }
    }

    protected override void UseAbility2()
    {
        if (ability2CooldownTimer <= 0f)
        {
            StartCoroutine(HeatwaveDash());
            ability2CooldownTimer = abilities.ability2.cooldown / (PowerSurgeActive ? 2f : 1f);
        }
        else
        {
            Debug.LogWarning("Heatwave Dash is still on cooldown!");
        }
    }

    protected override void UseUltimate()
    {
        if (ultimateCooldownTimer <= 0f)
        {
            StartCoroutine(InfernalCage());
            ultimateCooldownTimer = abilities.ultimate.cooldown / (PowerSurgeActive ? 2f : 1f);
        }
        else
        {
            Debug.LogWarning("Infernal Cage is still on cooldown!");
        }
    }

    private IEnumerator HeatwaveDash()
    {
        float dashDistance = 5f;
        float dashDuration = 0.5f;
        float trailInterval = 0.05f;

        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + transform.forward * dashDistance;

        float elapsed = 0f;

        while (elapsed < dashDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / dashDuration);

            if (abilities.ability2.projectilePrefab != null)
            {
                Vector3 spawnPos = transform.position + Vector3.up * 0.2f;
                GameObject trail = Instantiate(abilities.ability2.projectilePrefab, spawnPos, Quaternion.identity);
                Destroy(trail, 2f);

                // Apply damage on contact during dash
                Collider[] enemies = Physics.OverlapSphere(transform.position, 1.5f);
                foreach (var enemy in enemies)
                {
                    if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
                    {
                        enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ability2.damage);
                        enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ability2.damage);
                    }
                }
            }
            else
            {
                Debug.LogError("Heatwave trail prefab not assigned!");
            }

            elapsed += trailInterval;
            yield return new WaitForSeconds(trailInterval);
        }
    }

    private IEnumerator InfernalCage()
    {
        int pillarCount = 8;
        float radius = 3f;
        List<GameObject> pillars = new List<GameObject>();

        for (int i = 0; i < pillarCount; i++)
        {
            float angle = i * Mathf.PI * 2f / pillarCount;
            Vector3 spawnPos = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius + Vector3.up * 0.5f;

            if (abilities.ultimate.projectilePrefab != null)
            {
                GameObject pillar = Instantiate(abilities.ultimate.projectilePrefab, spawnPos, Quaternion.identity);
                Destroy(pillar, 2f); 
                pillars.Add(pillar);
            }
            else
            {
                Debug.LogError("Infernal Cage pillar prefab not assigned!");
            }
        }

        yield return new WaitForSeconds(0.3f);

        foreach (var pillar in pillars)
        {
            if (pillar != null)
            {
                Collider[] enemies = Physics.OverlapSphere(pillar.transform.position, 2f);
                foreach (var enemy in enemies)
                {
                    if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
                    {
                        enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ultimate.damage);
                        enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ultimate.damage);
                        enemy.GetComponent<StatusEffects>()?.ApplyBurn(3f, 5);
                    }
                }
            }
        }
    }
}
