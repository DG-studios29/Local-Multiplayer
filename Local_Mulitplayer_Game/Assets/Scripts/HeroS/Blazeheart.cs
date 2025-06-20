// ------------------------
// Blazeheart.cs (Fixed)
// ------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Blazeheart : HeroBase, IPlayerEffect
{
    #region power-ups vars
    private int availableInstantCooldowns = 0;
    private TMP_Text availableCooldownsTxt;
    #endregion
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    protected override void UseAbility1()
    {
        if (ability1CooldownTimer <= 0f) StartCoroutine(UseAbility1Routine());
        else
        {
            if (availableInstantCooldowns > 0)
            {
                ability1CooldownTimer = 0f;
                availableInstantCooldowns--;
                StartCoroutine(OnCooldownUse());
            }
        }
    }

    private IEnumerator UseAbility1Routine()
    {
        animator?.SetTrigger("CastFire");

        // Wait for animation (can be matched with your actual animation length)
        yield return new WaitForSeconds(0.2f); // replace with exact animation length

        ShootProjectile(abilities.ability1);
        ability1CooldownTimer = abilities.ability1.cooldown / (PowerSurgeActive ? 2f : 1f);
    }

    protected override void UseAbility2()
    {
        if (ability2CooldownTimer <= 0f)
        {
            animator.SetTrigger("DashFlame");
            StartCoroutine(HeatwaveDash());
            ability2CooldownTimer = abilities.ability2.cooldown / (PowerSurgeActive ? 2f : 1f);
        }
        else
        {
            if (availableInstantCooldowns > 0)
            {
                ability2CooldownTimer = 0f;
                availableInstantCooldowns--;
                StartCoroutine(OnCooldownUse());
            }
        }
    }

    protected override void UseUltimate()
    {
        if (ultimateCooldownTimer <= 0f)
        {
            animator.SetTrigger("CastInferno");
            StartCoroutine(InfernalCage());
            ultimateCooldownTimer = abilities.ultimate.cooldown / (PowerSurgeActive ? 2f : 1f);
        }
        else
        {
            if (availableInstantCooldowns > 0)
            {
                ultimateCooldownTimer = 0f;
                availableInstantCooldowns--;
                StartCoroutine(OnCooldownUse());
            }
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
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;

            if (abilities.ability2.projectilePrefab)
            {
                GameObject trail = Instantiate(abilities.ability2.projectilePrefab, spawnPos, Quaternion.identity);
                Destroy(trail, 2f);
            }

            Collider[] enemies = Physics.OverlapSphere(transform.position, 1.5f);
            foreach (var enemy in enemies)
            {
                if (enemy == null || enemy.gameObject == null) continue;
                if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
                {
                    enemy.attachedRigidbody?.AddForce(Vector3.down * 5f, ForceMode.Impulse);
                    enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ability2.damage, gameObject);
                    enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ability2.damage, gameObject);
                }
            }
            elapsed += trailInterval;
            yield return new WaitForSeconds(trailInterval);
        }
    }

    private IEnumerator InfernalCage()
    {
        int pillarCount = 8;
        float radius = 5f;
        List<GameObject> pillars = new List<GameObject>();

        for (int i = 0; i < pillarCount; i++)
        {
            float angle = i * Mathf.PI * 2f / pillarCount;
            Vector3 spawnPos = transform.position + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius + Vector3.up * 0.5f;

            if (abilities.ultimate.projectilePrefab)
            {
                GameObject pillar = Instantiate(abilities.ultimate.projectilePrefab, spawnPos, Quaternion.identity);
                pillars.Add(pillar);
                Destroy(pillar, 5f);
            }
        }

        yield return new WaitForSeconds(0.25f);

        foreach (var pillar in pillars)
        {
            if (pillar != null)
            {
                Collider[] enemies = Physics.OverlapSphere(pillar.transform.position, 5f);
                foreach (var enemy in enemies)
                {
                    if (enemy == null || enemy.gameObject == null) continue;
                    if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
                    {
                        enemy.attachedRigidbody?.AddForce(Vector3.down * 5f, ForceMode.Impulse);
                        enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ultimate.damage, gameObject);
                        enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ultimate.damage, gameObject);
                        enemy.GetComponent<StatusEffects>()?.ApplyBurn(3f, 5);
                    }
                }
            }
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

    public void RestoreOrbs()
    {

    }

    public void ResetAbilityCooldownTimer(int cooldown)
    {
        availableInstantCooldowns += cooldown;
        StartCoroutine(UITime());
    }
    #endregion

    #region Custom Methods

    IEnumerator UITime()
    {
        switch (transform.root.name)
        {
            case "Player 1":
                GameManager.Instance.playerOnePowerUps[4].alpha = 1;
                availableCooldownsTxt = GameManager.Instance.playerOnePowerUps[4].transform.GetChild(0)
                    .gameObject.GetComponent<TMP_Text>();
                if (availableCooldownsTxt != null) availableCooldownsTxt.text = availableInstantCooldowns.ToString();
                break;

            case "Player 2":
                GameManager.Instance.playerTwoPowerUps[4].alpha = 1;
                availableCooldownsTxt = GameManager.Instance.playerTwoPowerUps[4].transform.GetChild(0)
                .gameObject.GetComponent<TMP_Text>();
                if (availableCooldownsTxt != null) availableCooldownsTxt.text = availableInstantCooldowns.ToString();
                break;
        }

        yield return new WaitForSeconds(1f);
        switch (transform.root.name)
        {
            case "Player 1":
                GameManager.Instance.playerOnePowerUps[4].alpha = 0.1f;
                break;

            case "Player 2":
                GameManager.Instance.playerTwoPowerUps[4].alpha = 0.1f;
                break;
        }
    }

    IEnumerator OnCooldownUse()
    {
        switch (transform.root.name)
        {
            case "Player 1":
                GameManager.Instance.playerOnePowerUps[4].alpha = 1;
                if (availableCooldownsTxt != null) availableCooldownsTxt.text = availableInstantCooldowns.ToString();
                break;

            case "Player 2":
                GameManager.Instance.playerTwoPowerUps[4].alpha = 1;
                if (availableCooldownsTxt != null) availableCooldownsTxt.text = availableInstantCooldowns.ToString();
                break;
        }

        yield return new WaitForSeconds(1f);
        switch (transform.root.name)
        {
            case "Player 1":
                GameManager.Instance.playerOnePowerUps[4].alpha = 0.1f;
                break;

            case "Player 2":
                GameManager.Instance.playerTwoPowerUps[4].alpha = 0.1f;
                break;
        }
    }
    #endregion
}
