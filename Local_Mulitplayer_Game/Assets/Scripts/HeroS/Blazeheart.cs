
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Blazeheart : HeroBase, IPlayerEffect
{
    private int availableInstantCooldowns = 0;

    protected override void UseAbility1()
    {
        if (ability1CooldownTimer <= 0f)
        {
            // Molten Shot - applies ignite and armor reduction
            ShootProjectile(abilities.ability1);
            ability1CooldownTimer = abilities.ability1.cooldown;
        }
        else
        {
            if (availableInstantCooldowns > 0)
            {
                ability1CooldownTimer = 0f;
                availableInstantCooldowns--;
            }

            Debug.LogWarning("Molten Shot is still on cooldown!");
        }
    }

    protected override void UseAbility2()
    {
        if (ability2CooldownTimer <= 0f)
        {
            Debug.Log("Blazeheart uses Heatwave Dash!");
            StartCoroutine(HeatwaveDash());
            ability2CooldownTimer = abilities.ability2.cooldown;
        }
        else
        {
            if (availableInstantCooldowns > 0)
            {
                ability2CooldownTimer = 0f;
                availableInstantCooldowns--;
            }

            Debug.LogWarning("Heatwave Dash is still on cooldown!");
        }
    }

    protected override void UseUltimate()
    {
        if (ultimateCooldownTimer <= 0f)
        {
            Debug.Log("Blazeheart casts Infernal Cage!");
            StartCoroutine(InfernalCage());
            ultimateCooldownTimer = abilities.ultimate.cooldown;
        }
        else
        {
            if (availableInstantCooldowns > 0)
            {
                ultimateCooldownTimer = 0f;
                availableInstantCooldowns--;
            }

            Debug.LogWarning("Infernal Cage is still on cooldown!");
        }
    }

    private IEnumerator HeatwaveDash()
    {
        float dashDistance = 7f;
        float dashDuration = 0.5f;
        float trailDuration = 3f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = startPosition + transform.forward * dashDistance;

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / dashDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        GameObject trail = Instantiate(abilities.ability2.projectilePrefab, transform.position, Quaternion.identity);
        Destroy(trail, trailDuration);

        Collider[] enemies = Physics.OverlapSphere(transform.position, 3f);
        foreach (var enemy in enemies)
        {
            if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
            {
                enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ability2.damage);
                enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ability2.damage);

                var status = enemy.GetComponent<StatusEffects>();
                if (status != null)
                {
                    status.ApplyBurn(3f, 5);
                    status.ApplySlow(2f, 0.5f);
                }
            }
        }
    }

    private IEnumerator InfernalCage()
    {
        float duration = 6f;
        float radius = 6f;
        int pillarCount = 8;

        List<GameObject> pillars = new List<GameObject>();
        for (int i = 0; i < pillarCount; i++)
        {
            float angle = i * Mathf.PI * 2f / pillarCount;
            Vector3 pos = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            GameObject pillar = Instantiate(abilities.ultimate.projectilePrefab, pos, Quaternion.identity);
            pillars.Add(pillar);
        }

        yield return new WaitForSeconds(duration);

        foreach (GameObject pillar in pillars)
        {
            Collider[] enemies = Physics.OverlapSphere(pillar.transform.position, 3f);
            foreach (var enemy in enemies)
            {
                if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
                {
                    enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ultimate.damage);
                    enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ultimate.damage);

                    var status = enemy.GetComponent<StatusEffects>();
                    if (status != null)
                        status.ApplyBurn(3f, 10);
                }
            }
            Destroy(pillar);
        }
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
