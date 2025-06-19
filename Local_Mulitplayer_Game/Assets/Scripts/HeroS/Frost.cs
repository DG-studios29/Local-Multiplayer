
using UnityEngine;
using System.Collections;
using TMPro;

public class Frost : HeroBase, IPlayerEffect
{
    #region power-ups vars
    private int availableInstantCooldowns = 0;
    private TMP_Text availableCooldownsTxt;
    #endregion

    protected override void UseAbility1()
    {
        if (ability1CooldownTimer <= 0f)
        {
            ShootProjectile(abilities.ability1);
            ability1CooldownTimer = abilities.ability1.cooldown / (PowerSurgeActive ? 2f : 1f);
        }
        else
        {
            if (availableInstantCooldowns > 0)
            {
                ability1CooldownTimer = 0f;
                availableInstantCooldowns--;
                StartCoroutine(OnCooldownUse());
            }

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
            if (availableInstantCooldowns > 0)
            {
                ability2CooldownTimer = 0f;
                availableInstantCooldowns--;
                StartCoroutine(OnCooldownUse());
            }

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
            if (availableInstantCooldowns > 0)
            {
                ultimateCooldownTimer = 0f;
                availableInstantCooldowns--;
                StartCoroutine(OnCooldownUse());
            }
            Debug.LogWarning("Absolute Zero is still on cooldown!");
        }
    }

    private IEnumerator FrozenWall()
    {
        GameObject wall = Instantiate(abilities.ability2.projectilePrefab, transform.position + transform.forward * 2f, Quaternion.identity);

        yield return new WaitForSeconds(5f);

        if (wall != null)
        {
            Collider[] enemies = Physics.OverlapSphere(wall.transform.position, 5f);
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

        Collider[] enemies = Physics.OverlapSphere(transform.position, 8f);
        foreach (var enemy in enemies)
        {
            if (enemy == null || enemy.gameObject == null) continue;
            if ((enemy.CompareTag("Enemy") || enemy.CompareTag("Player")) && enemy.gameObject != gameObject)
            {
                var status = enemy.GetComponent<StatusEffects>();
                status?.ApplyStun(5f);
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
