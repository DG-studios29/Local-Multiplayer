
using System.Collections;
using UnityEngine;

public class Nightshade : HeroBase
{
    protected override void UseAbility1()
    {
        if (ability1CooldownTimer <= 0f)
        {
            // Shadow Bolt - marks enemy for bonus damage
            ShootProjectile(abilities.ability1);
            ability1CooldownTimer = abilities.ability1.cooldown;
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
            Debug.Log("Nightshade deploys a Phantom Clone!");
            StartCoroutine(PhantomClone());
            ability2CooldownTimer = abilities.ability2.cooldown;
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
            Debug.Log("Nightshade unleashes Grave Silence!");
            StartCoroutine(GraveSilence());
            ultimateCooldownTimer = abilities.ultimate.cooldown;
        }
        else
        {
            Debug.LogWarning("Grave Silence is still on cooldown!");
        }
    }

    private IEnumerator PhantomClone()
    {
        GameObject clone = Instantiate(abilities.ability2.projectilePrefab, transform.position, transform.rotation);
        clone.transform.localScale = transform.localScale;

        float cloneDuration = 3f;
        yield return new WaitForSeconds(cloneDuration);

        Collider[] hit = Physics.OverlapSphere(clone.transform.position, 3f);
        foreach (var enemy in hit)
        {
            if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
            {
                enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ability2.damage);
                enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ability2.damage);

                var status = enemy.GetComponent<StatusEffects>();
                if (status != null)
                    status.ApplySilence(2f);
            }
        }

        Destroy(clone);
    }

    private IEnumerator GraveSilence()
    {
        float radius = 6f;
        float blindDuration = 2f;

        Collider[] enemies = Physics.OverlapSphere(transform.position, radius);
        foreach (var enemy in enemies)
        {
            if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
            {
                var status = enemy.GetComponent<StatusEffects>();
                if (status != null)
                {
                    status.ApplySilence(3f);
                    status.ApplySlow(2f, 0.5f);
                }

                // Shadow Spike
                enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ultimate.damage);
                enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ultimate.damage);
            }
        }

        yield return new WaitForSeconds(blindDuration);
    }
}
