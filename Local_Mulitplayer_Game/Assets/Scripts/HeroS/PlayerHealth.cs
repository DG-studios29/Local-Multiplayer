using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;
using System.Collections;

public class PlayerHealth : MonoBehaviour, IPlayerEffect
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

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
    
    


    #region Interface Vars

    private bool isShielded = false;
    private bool hasShieldBubble;
    private Coroutine shieldCoroutine;
    private Coroutine healthCoroutine;
    
    #endregion

    private void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Start()
    {
        playerMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        baseMaterial = playerMeshRenderers[0].material;
        StartCoroutine(ValidatePlayer());
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

        if(!alreadyHurting && !isFrozen)
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
        // 🔹 You can add respawn logic or player death effects here
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
        foreach(var part in playerMeshRenderers)
        {
            part.material = materialStatus;
        }
    }

    

    #region Interfaces

    public void ActivateSpeedBoost(float duration, float speedMultiplier, GameObject trailEffect)
    {
        //throw new System.NotImplementedException();
    }

    public void ActivateShield(float duration, GameObject shield)
    {
        isShielded = true;

        if (!hasShieldBubble)
        {
            shield = Instantiate(shield);
            hasShieldBubble = true;
        }

        shield.transform.SetParent(transform);
        shield.transform.localPosition = new Vector3(0, 0.5f, 0);
        shield.transform.localRotation = Quaternion.identity;
        shield.transform.localScale = new Vector3(.77f, .7f, .7f);

        if (shieldCoroutine != null) StopCoroutine(shieldCoroutine);
        shieldCoroutine = StartCoroutine(ShieldTime(duration, shield));

        switch (isPlayer)
        {
            case IsPlayer.PlayerOne:
                GameManager.Instance.playerOnePowerUps[1].alpha = 1f;
                break;

            case IsPlayer.PlayerTwo:
                GameManager.Instance.playerTwoPowerUps[1].alpha = 1f;
                break;
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
        yield return new WaitForSeconds(3f);
        switch (isPlayer)
        {
            case IsPlayer.PlayerOne:
                GameManager.Instance.playerOnePowerUps[0].alpha = 0.4f;
                break;

            case IsPlayer.PlayerTwo:
                GameManager.Instance.playerTwoPowerUps[0].alpha = 0.4f;
                break;
        }
    }

    public void RefillAbilityBar(float energy)
    {
        //throw new System.NotImplementedException();
    }

    private IEnumerator ShieldTime(float duration, GameObject shieldBubble)
    {
        yield return new WaitForSeconds(duration);

        isShielded = false;

        if (shieldBubble != null)
        {
            Destroy(shieldBubble);
        }

        hasShieldBubble = false;

        switch (isPlayer)
        {
            case IsPlayer.PlayerOne:
                GameManager.Instance.playerOnePowerUps[1].alpha = 0.4f;
                break;

            case IsPlayer.PlayerTwo:
                GameManager.Instance.playerTwoPowerUps[1].alpha = 0.4f;
                break;
        }
    }
    #endregion
}
