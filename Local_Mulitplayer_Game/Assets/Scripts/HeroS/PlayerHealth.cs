using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour, IPlayerEffect
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;
    public static bool SuddenDeathActive = false;
    public static bool NoHealingActive = false;



    [Header("UI Elements")]
    public Slider healthSlider;
    public TMP_Text healthText;


    private bool isFrozen;
    private float freezeDuration;

    public enum IsPlayer { PlayerOne, PlayerTwo }
    public IsPlayer isPlayer;

    //show material change
    [SerializeField] private Material frozenMaterial;
    [SerializeField] private Material hurtMaterial;
    private float hurtTime = 0.25f;
    bool alreadyHurting = false;
    private Material baseMaterial;
    private MeshRenderer[] playerMeshRenderers;
    private int preSuddenDeathHealth = -1;




    #region Interface Vars
    private GameObject activeShield;
    private Image shieldImage;
    private float currentShieldTimer = 0;
    private bool isShielded = false;
    private Coroutine shieldCoroutine;
    private Coroutine healthCoroutine;

    #endregion

    private void Start()
    {
        playerMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        baseMaterial = playerMeshRenderers[0].material;
        StartCoroutine(ValidatePlayer());
        currentHealth = maxHealth;
        UpdateHealthUI();

        ArenaEventManager.OnArenaEventStart += HandleArenaEvent;
        ArenaEventManager.OnArenaEventEnd += HandleArenaEventEnd; 
    }

    private void OnDestroy()
    {
        ArenaEventManager.OnArenaEventStart -= HandleArenaEvent;
        ArenaEventManager.OnArenaEventEnd -= HandleArenaEventEnd; 
    }



    private IEnumerator ValidatePlayer()
    {
        yield return new WaitForSeconds(5f);
        if (gameObject.name == "Player 1") isPlayer = IsPlayer.PlayerOne;
        if (gameObject.name == "Player 2") isPlayer = IsPlayer.PlayerTwo;
    }

    public void TakeDamage(int damage)
    {
        if (isShielded) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI(); // 🔹 Ensure UI updates every time damage is taken

        Debug.Log($"{gameObject.name} took {damage} damage. Current Health: {currentHealth}");

        if (!alreadyHurting && !isFrozen)
            StartCoroutine(ShowHurt());

        if (currentHealth <= 0)
        {
            Die();
        }

    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();

        if (NoHealingActive)
        {
            Debug.Log("Healing blocked due to No Healing arena event!");
            return;
        }
    }

    public void UpdateHealthUI()
    {
        if (healthSlider != null)
            healthSlider.value = (float)currentHealth / maxHealth; // 🔹 Ensure slider updates

        if (healthText != null)
            healthText.text = $"{currentHealth} / {maxHealth}"; // 🔹 Ensure text updates
    }

    void Die()
    {
        Debug.Log(gameObject.name + " has died!");

        if (ArenaEventManager.Instance != null && ArenaEventManager.Instance.IsActive("triggerPlayerRespawner"))
        {
            StartCoroutine(RespawnAfterDelay(2f));
        }

        Debug.Log(gameObject.name + " has died!");

    }

    public void Freeze(float duration)
    {
        if (isFrozen) return; // Prevent freezing if already frozen
        isFrozen = true;
        freezeDuration = duration;

        StartCoroutine(FreezeDuration());
    }

    private IEnumerator FreezeDuration()
    {
        baseMaterial = playerMeshRenderers[0].material;
        
        ChangeMat(frozenMaterial);
        yield return new WaitForSeconds(freezeDuration);
        isFrozen = false;
        ChangeMat(baseMaterial);
    }


    private IEnumerator ShowHurt()
    {
        baseMaterial = playerMeshRenderers[0].material;
        
        alreadyHurting = true;
        yield return new WaitForSeconds(0.1f);


        ChangeMat(hurtMaterial);

        yield return new WaitForSeconds(hurtTime);

        ChangeMat(baseMaterial);
        alreadyHurting = false;
    }

    private void ChangeMat(Material materialStatus)
    {
        foreach (var part in playerMeshRenderers)
        {
            part.material = materialStatus;
        }
    }
    private void HandleArenaEvent(ArenaEventSO evt)
    {
        if (evt.triggerSuddenDeath)
        {
            SuddenDeathActive = true;

            // Save current health to restore later
            preSuddenDeathHealth = currentHealth;

            // Drop player to 1 HP
            currentHealth = 1;
            UpdateHealthUI();
        }
    }

    private void HandleArenaEventEnd(ArenaEventSO evt)
    {
        if (evt.triggerSuddenDeath)
        {
            SuddenDeathActive = false;

            // Restore saved health only if still alive
            if (currentHealth > 0 && preSuddenDeathHealth > 0)
            {
                currentHealth = preSuddenDeathHealth;
                UpdateHealthUI();
            }

            // Clear saved value
            preSuddenDeathHealth = -1;
        }
    }

    private IEnumerator RespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentHealth = maxHealth;
        UpdateHealthUI();
        Debug.Log($"{gameObject.name} has been auto-respawned!");
    }






    #region Interfaces

    public void ActivateSpeedBoost(float duration, float speedMultiplier, GameObject trailEffect)
    {

    }

    public void ActivateShield(float duration, GameObject shield)
    {
        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
            shieldCoroutine = null;
        }

        if (activeShield != null)
        {
            Destroy(activeShield);
        }

        activeShield = Instantiate(shield, transform);
        activeShield.transform.localPosition = new Vector3(0, 0.5f, 0);
        activeShield.transform.localRotation = Quaternion.identity;
        activeShield.transform.localScale = new Vector3(.7f, .7f, .7f);

        isShielded = true;

        shieldCoroutine = StartCoroutine(ShieldTime(duration));


        switch (isPlayer)
        {
            case IsPlayer.PlayerOne:
                GameManager.Instance.playerOnePowerUps[1].alpha = 1f;
                shieldImage = GameManager.Instance.playerOnePowerUps[1].gameObject.GetComponent<Image>();
                break;

            case IsPlayer.PlayerTwo:
                GameManager.Instance.playerTwoPowerUps[1].alpha = 1f;
                shieldImage = GameManager.Instance.playerTwoPowerUps[1].gameObject.GetComponent<Image>();
                break;
        }
    }

    private IEnumerator ShieldTime(float duration)
    {
        yield return StartCoroutine(CountHelper(duration));

        isShielded = false;
        currentShieldTimer = 0;
        if (activeShield != null)
        {
            Destroy(activeShield);
        }

        switch (isPlayer)
        {
            case IsPlayer.PlayerOne:
                GameManager.Instance.playerOnePowerUps[1].alpha = 0.1f;
                break;

            case IsPlayer.PlayerTwo:
                GameManager.Instance.playerTwoPowerUps[1].alpha = 0.1f;
                break;
        }
    }

    private IEnumerator CountHelper(float dur)
    {
        currentShieldTimer = dur;
        while (currentShieldTimer > 0)
        {
            currentShieldTimer -= Time.deltaTime;

            if (shieldImage != null)
            {
                shieldImage.fillAmount = currentShieldTimer / dur;
            }

            yield return null;
        }
    }

    public void GiveHealth(float health)
    {
        currentHealth += (int)health;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
        if (healthCoroutine != null) StopCoroutine(healthCoroutine);
        healthCoroutine = StartCoroutine(healthtime());


        switch (isPlayer)
        {
            case IsPlayer.PlayerOne:
                GameManager.Instance.playerOnePowerUps[0].alpha = 1f;
                break;

            case IsPlayer.PlayerTwo:
                GameManager.Instance.playerTwoPowerUps[0].alpha = 1f;
                break;
        }
    }

    IEnumerator healthtime()
    {
        yield return new WaitForSeconds(1f);
        switch (isPlayer)
        {
            case IsPlayer.PlayerOne:
                GameManager.Instance.playerOnePowerUps[0].alpha = 0.1f;
                break;

            case IsPlayer.PlayerTwo:
                GameManager.Instance.playerTwoPowerUps[0].alpha = 0.1f;
                break;
        }
    }

    public void RestoreOrbs()
    {
        //
    }


    public void ResetAbilityCooldownTimer(int cooldown)
    {
        //
    }
    #endregion
}
