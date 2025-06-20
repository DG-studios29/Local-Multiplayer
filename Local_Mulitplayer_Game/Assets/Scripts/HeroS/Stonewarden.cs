
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Stonewarden : HeroBase, IPlayerEffect
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

            Debug.LogWarning("Stone Fist is still on cooldown!");
        }
    }
    private IEnumerator UseAbility1Routine()
    {
        animator?.SetTrigger("PunchStone");

        // Wait for animation (can be matched with your actual animation length)
        yield return new WaitForSeconds(0.2f); // replace with exact animation length

        ShootProjectile(abilities.ability1);
        ability1CooldownTimer = abilities.ability1.cooldown / (PowerSurgeActive ? 2f : 1f);
    }

    protected override void UseAbility2()
    {
        if (ability2CooldownTimer <= 0f)
        {
            animator?.SetTrigger("RockArmor");
            StartCoroutine(Fortify());
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
            if (availableInstantCooldowns > 0)
            {
                ultimateCooldownTimer = 0f;
                availableInstantCooldowns--;
                StartCoroutine(OnCooldownUse());
            }

            Debug.LogWarning("Seismic Rift is still on cooldown!");
        }
    }

    private IEnumerator Fortify()
    {
        var status = GetComponent<StatusEffects>();
        status?.ApplyArmorBuff(5f, 0.5f); // 50% damage reduction

        // === VISUAL TRANSFORMATION START ===
        if (abilities.ability2.projectilePrefab != null)
        {
            GameObject rockForm = Instantiate(abilities.ability2.projectilePrefab, transform);
            rockForm.transform.localPosition = Vector3.zero;
            rockForm.transform.localRotation = Quaternion.identity;

            // Optionally hide original visuals
            SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var rend in renderers) rend.enabled = false;

            yield return new WaitForSeconds(5f);

            Destroy(rockForm); // Remove rock overlay

            // Restore visuals
            foreach (var rend in renderers) rend.enabled = true;
        }
        else
        {
            Debug.LogWarning("Fortify rock form prefab not assigned.");
            yield return new WaitForSeconds(5f);
        }
    }


    private IEnumerator SeismicRift()
    {
        int segments = 6;
        float delayBetweenSegments = 0.2f;
        float range = 7f;
        float segmentSpacing = range / segments;
        Vector3 direction = transform.forward;

        animator?.SetTrigger("SmashGround");
        yield return new WaitForSeconds(0.7f);
        for (int i = 1; i <= segments; i++)
        {
            
            Vector3 segmentPosition = transform.position + direction * (i * segmentSpacing);
            GameObject riftSegment = Instantiate(abilities.ultimate.projectilePrefab, segmentPosition, Quaternion.identity);

            Collider[] enemies = Physics.OverlapBox(segmentPosition, new Vector3(2f, 2f, 2f));
            foreach (var enemy in enemies)
            {
                if (enemy == null || enemy.gameObject == null) continue;
                if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
                {
                    enemy.GetComponent<PlayerHealth>()?.TakeDamage((int)abilities.ultimate.damage, gameObject);
                    enemy.GetComponent<EnemyAI>()?.TakeDamage((int)abilities.ultimate.damage, gameObject);
                    enemy.GetComponent<StatusEffects>()?.ApplyStun(1f);
                }
            }

            yield return new WaitForSeconds(delayBetweenSegments);
            Destroy(riftSegment, 3f); 
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
