
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public abstract class HeroBase : MonoBehaviour
{
    public HeroAbility abilities;
    private PlayerInput playerInput;
    private Transform projectileSpawnPoint;
    private float projectileSpeed = 20f;
    public static bool PowerSurgeActive = false;

    public float ability1CooldownTimer = 0f;
    public float ability2CooldownTimer = 0f;
    public float ultimateCooldownTimer = 0f;

    private AutoAttack autoattack;

    public TMP_Text ability1CooldownText;
    public TMP_Text ability2CooldownText;
    public TMP_Text ultimateCooldownText;

    public Image ability1Icon;
    public Image ability2Icon;
    public Image ultimateIcon;

    public int casterID;
    private Color originalAbility1Color;
    private Color originalAbility2Color;
    private Color originalUltimateColor;

    protected virtual void Start()
    {
        casterID = gameObject.GetInstanceID();
        ArenaEventManager.OnArenaEventStart += HandleArenaEvent;
        ArenaEventManager.OnArenaEventEnd += HandleArenaEventEnd;

        if (ability1Icon) originalAbility1Color = ability1Icon.color;
        if (ability2Icon) originalAbility2Color = ability2Icon.color;
        if (ultimateIcon) originalUltimateColor = ultimateIcon.color;

        playerInput = GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.actions["Ability1"].performed += ctx =>
            {
                if (!PlayerPunches.OnlyPunchesActive) UseAbility1();
            };
            playerInput.actions["Ability2"].performed += ctx =>
            {
                if (!PlayerPunches.OnlyPunchesActive) UseAbility2();
            };
            playerInput.actions["Ultimate"].performed += ctx =>
            {
                if (!PlayerPunches.OnlyPunchesActive) UseUltimate();
            };
        }

        projectileSpawnPoint = transform.Find("ProjectileSpawnPoint");
        if (!projectileSpawnPoint)
        {
            Debug.LogError($"{gameObject.name} is missing a ProjectileSpawnPoint!");
        }

        autoattack = GetComponentInChildren<AutoAttack>();
        if (autoattack != null && abilities != null)
        {
            autoattack.InstantiateRevolver(abilities.itemRevolve);
        }
    }

    protected abstract void UseAbility1();
    protected abstract void UseAbility2();
    protected abstract void UseUltimate();

    public void ShootProjectile(Ability ability)
    {
        if (ability.projectilePrefab == null)
        {
            Debug.LogError($"{ability.abilityName} projectile prefab is missing! Assign it in the HeroAbility scriptable object.");
            return;
        }

        Vector3 spawnPosition = projectileSpawnPoint.position + transform.forward * 1f;

        GameObject projectile = Instantiate(ability.projectilePrefab, spawnPosition, Quaternion.identity);

        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        if (rb != null)
        {
            Vector3 direction = transform.forward;
            rb.linearVelocity = direction * projectileSpeed;
        }

        Projectile projScript = projectile.GetComponent<Projectile>();
        if (projScript != null)
        {
            projScript.Initialize(gameObject, ability.damage);
        }
    }

    private void Update()
    {
        if (ability1Icon == null || ability2Icon == null || ultimateIcon == null ||
            ability1CooldownText == null || ability2CooldownText == null || ultimateCooldownText == null)
            return;

        UpdateCooldowns();

        UpdateAbilityUI(ability1CooldownTimer, ability1Icon, ability1CooldownText);
        UpdateAbilityUI(ability2CooldownTimer, ability2Icon, ability2CooldownText);
        UpdateAbilityUI(ultimateCooldownTimer, ultimateIcon, ultimateCooldownText);
    }

    void UpdateCooldowns()
    {
        float cooldownRate = PowerSurgeActive ? 2f : 1f;

        if (ability1CooldownTimer > 0)
            ability1CooldownTimer -= Time.deltaTime * cooldownRate;

        if (ability2CooldownTimer > 0)
            ability2CooldownTimer -= Time.deltaTime * cooldownRate;

        if (ultimateCooldownTimer > 0)
            ultimateCooldownTimer -= Time.deltaTime * cooldownRate;
    }

    void UpdateAbilityUI(float cooldownTimer, Image abilityIcon, TMP_Text cooldownText)
    {
        if (abilityIcon == null || cooldownText == null)
            return;

        if (cooldownTimer > 0)
        {
            abilityIcon.color = Color.gray;
            cooldownText.gameObject.SetActive(true);
            cooldownText.text = cooldownTimer.ToString("F0");
        }
        else
        {
            cooldownText.gameObject.SetActive(false);

            if (abilityIcon == ability1Icon)
                abilityIcon.color = originalAbility1Color;
            else if (abilityIcon == ability2Icon)
                abilityIcon.color = originalAbility2Color;
            else if (abilityIcon == ultimateIcon)
                abilityIcon.color = originalUltimateColor;
        }
    }

    private void HandleArenaEvent(ArenaEventSO evt)
    {
        if (evt.triggerPowerSurge)
            PowerSurgeActive = true;

        if (evt.triggerOnlyPunches)
            PlayerPunches.OnlyPunchesActive = true;
    }

    private void HandleArenaEventEnd(ArenaEventSO evt)
    {
        if (evt.triggerPowerSurge)
            PowerSurgeActive = false;

        if (evt.triggerOnlyPunches)
            PlayerPunches.OnlyPunchesActive = false;
    }

    public void ResetCooldowns()
    {
        ability1CooldownTimer = abilities.ability1.cooldown;
        ability2CooldownTimer = abilities.ability2.cooldown;
        ultimateCooldownTimer = abilities.ultimate.cooldown;
    }

    private void OnDestroy()
    {
        ArenaEventManager.OnArenaEventStart -= HandleArenaEvent;
        ArenaEventManager.OnArenaEventEnd -= HandleArenaEventEnd;
    }
}
