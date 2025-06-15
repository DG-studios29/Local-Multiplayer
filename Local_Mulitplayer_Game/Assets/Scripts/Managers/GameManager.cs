using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Player Settings")]
    public List<string> selectedHeroes = new List<string>();
    public List<Material> playerMaterials;
    public List<GameObject> activePlayers = new List<GameObject>();

    [Header("Spawn Points")]
    public List<Transform> forestSpawnPoints;
    public List<Transform> cemeterySpawnPoints;
    public List<Transform> winterSpawnPoints;
    private List<Transform> currentSpawnPoints;

    [Header("Maps")]
    public string selectedMap = "Forest";
    public GameObject forestMap;
    public GameObject cemeteryMap;
    public GameObject wintermap;

    [Header("Camera")]
    public CinemachineTargetGroup targetGroup;

    [Header("Player UI")]
    public Slider player1HealthSlider;
    public TMP_Text player1HealthText;
    public Slider player2HealthSlider;
    public TMP_Text player2HealthText;
    public TMP_Text player1NameText;
    public TMP_Text player2NameText;

    [Header("Resources UI")] 
    public CurrencyUI p1CurrencyUI;
    public CurrencyUI p2CurrencyUI;

    [Header("Army UI Panels")]
    public MiniArmySpawnerUI player1ArmyUI;
    public MiniArmySpawnerUI player2ArmyUI;

    [Header("Hero Abilities UI")]
    public Image player1Ability1Icon;
    public TMP_Text player1Ability1CooldownText;
    public Image player2Ability1Icon;
    public TMP_Text player2Ability1CooldownText;
    public Image player1Ability2Icon;
    public TMP_Text player1Ability2CooldownText;
    public Image player2Ability2Icon;
    public TMP_Text player2Ability2CooldownText;
    public Image player1Ability3Icon;
    public TMP_Text player1Ability3CooldownText;
    public Image player2Ability3Icon;
    public TMP_Text player2Ability3CooldownText;

    [Header("Player Active Powerups")]
    public CanvasGroup[] playerOnePowerUps = new CanvasGroup[3];
    public CanvasGroup[] playerTwoPowerUps = new CanvasGroup[3];

    [Header("Game Timer")]
    public float gameDuration = 300f;
    private float timer;
    private bool gameStarted = false;
    //private bool shakeTriggered = false;
    public TextMeshProUGUI timerText;

    private void Awake()
    {
        Instance = this;

        var allPlayers = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        foreach (var input in allPlayers)
        {
            Destroy(input.gameObject);
        }
    }

    private void Start()
    {
        HeroSelectionUI.Instance.Setup(2); // Hardcoded for now
    }

    private void Update()
    {
        if (gameStarted)
        {
            HandleGameTimer();
        }
    }

    void HandleGameTimer()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerUI();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    public void StartGame(List<string> chosenHeroes)
    {
        selectedHeroes = chosenHeroes;

        for (int i = 0; i < selectedHeroes.Count; i++)
        {
            if (string.IsNullOrEmpty(selectedHeroes[i]))
                selectedHeroes[i] = "Blazeheart";
        }

        timer = gameDuration;
        //shakeTriggered = false;
        gameStarted = true;

        SelectMap(selectedMap);

        var players = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        for (int i = 0; i < players.Length; i++)
        {
            PlayerInput input = players[i];
            GameObject player = input.gameObject;
            int index = input.playerIndex;

            Vector3 spawnPos = currentSpawnPoints.Count > index
                ? currentSpawnPoints[index].position
                : Vector3.zero;

            player.transform.position = spawnPos + Vector3.up * 0.5f;

            player.name = $"Player {index + 1}";

            MiniArmySpawner spawner = player.GetComponent<MiniArmySpawner>();
            if (spawner != null)
            {
                if (player.name == "Player 1" && player1ArmyUI != null)
                {
                    spawner.InitializeUI(player1ArmyUI);
                }
                else if (player.name == "Player 2" && player2ArmyUI != null)
                {
                    spawner.InitializeUI(player2ArmyUI);
                }
                else
                {
                    Debug.LogError($"❌ Could not assign Army UI to {player.name}! Missing reference in GameManager.");
                }
            }

            var playerCurrency = player.GetComponent<PlayerCurrency>();
            if (player.name == "Player 1" && p1CurrencyUI != null)
            {
                playerCurrency.SetupManaUI(p1CurrencyUI);
            }
            else if (player.name == "Player 2" && p2CurrencyUI != null)
            {
                playerCurrency.SetupManaUI(p2CurrencyUI);
            }
    
            AssignHeroScript(player, selectedHeroes[index]);
            AssignPlayerMaterials(player, index);
            SetupPlayerUI(player, player.name);
            SetupCurrency(player);
            SetupHeroAbilitiesUI(player);
            AddPlayerToCamera(player);
        }

        HeroSelectionUI.Instance.playerUICanvas.SetActive(true);
        Debug.Log("[GameManager] Game started.");
    }

    public void SelectMap(string mapName)
    {
        forestMap.SetActive(false);
        cemeteryMap.SetActive(false);
        wintermap.SetActive(false);

        switch (mapName)
        {
            case "Forest": forestMap.SetActive(true); currentSpawnPoints = forestSpawnPoints; break;
            case "Cemetery": cemeteryMap.SetActive(true); currentSpawnPoints = cemeterySpawnPoints; break;
            case "Winter": wintermap.SetActive(true); currentSpawnPoints = winterSpawnPoints; break;
        }
    }

    public void AssignHeroScript(GameObject player, string heroName)
    {
        var existing = player.GetComponent<HeroBase>();
        if (existing != null) Destroy(existing);

        HeroBase hero = null;
        HeroAbility data = Resources.Load<HeroAbility>($"Abilities/{heroName}");

        switch (heroName)
        {
            case "Blazeheart": hero = player.AddComponent<Blazeheart>(); break;
            case "Frost": hero = player.AddComponent<Frost>(); break;
            case "Nightshade": hero = player.AddComponent<Nightshade>(); break;
            case "Stonewarden": hero = player.AddComponent<Stonewarden>(); break;
        }

        if (hero != null && data != null)
            hero.abilities = data;
    }

    public void AssignPlayerMaterials(GameObject player, int index)
    {
        if (selectedHeroes.Count <= index) return;

        string heroName = selectedHeroes[index];
        Material mat = playerMaterials.Find(m => m.name.StartsWith(heroName));
        if (mat == null) return;

        foreach (var r in player.GetComponentsInChildren<MeshRenderer>())
            r.material = mat;
    }

    public void SetupPlayerUI(GameObject player, string playerName)
    {
        var hp = player.GetComponent<PlayerHealth>();
        if (player.name == "Player 1")
        {
            hp.healthSlider = player1HealthSlider;
            hp.healthText = player1HealthText;
            player1NameText.text = playerName;
        }
        else if (player.name == "Player 2")
        {
            hp.healthSlider = player2HealthSlider;
            hp.healthText = player2HealthText;
            player2NameText.text = playerName;
        }
        hp.UpdateHealthUI();
    }


    private void SetupCurrency(GameObject player)
    {
        var playerCurrency = player.GetComponent<PlayerCurrency>();
        if (player.name == "Player 1" && p1CurrencyUI)
        {
            playerCurrency.SetupManaUI(p1CurrencyUI);
        }
        else if (player.name == "Player 2" && p2CurrencyUI)
        {
            playerCurrency.SetupManaUI(p2CurrencyUI);
        }
    }

    public void SetupHeroAbilitiesUI(GameObject player)
    {
        var hero = player.GetComponent<HeroBase>();
        if (hero == null || hero.abilities == null) return;

        if (player.name == "Player 1")
        {
            hero.ability1Icon = player1Ability1Icon;
            hero.ability2Icon = player1Ability2Icon;
            hero.ultimateIcon = player1Ability3Icon;

            player1Ability1Icon.sprite = hero.abilities.ability1.icon;
            player1Ability2Icon.sprite = hero.abilities.ability2.icon;
            player1Ability3Icon.sprite = hero.abilities.ultimate.icon;

            hero.ability1CooldownText = player1Ability1CooldownText;
            hero.ability2CooldownText = player1Ability2CooldownText;
            hero.ultimateCooldownText = player1Ability3CooldownText;
        }
        else if (player.name == "Player 2")
        {
            hero.ability1Icon = player2Ability1Icon;
            hero.ability2Icon = player2Ability2Icon;
            hero.ultimateIcon = player2Ability3Icon;

            player2Ability1Icon.sprite = hero.abilities.ability1.icon;
            player2Ability2Icon.sprite = hero.abilities.ability2.icon;
            player2Ability3Icon.sprite = hero.abilities.ultimate.icon;

            hero.ability1CooldownText = player2Ability1CooldownText;
            hero.ability2CooldownText = player2Ability2CooldownText;
            hero.ultimateCooldownText = player2Ability3CooldownText;
        }
    }

    public void RegisterPlayer(PlayerInput playerInput)
    {
        GameObject player = playerInput.gameObject;
        int index = playerInput.playerIndex;

        // Optional: position player here if needed
        if (index < forestSpawnPoints.Count)
        {
            player.transform.position = forestSpawnPoints[index].position;
        }

        player.name = "Player " + (index + 1);

        if (!activePlayers.Contains(player))
            activePlayers.Add(player);

        AddPlayerToCamera(player);
    }


    public void AddPlayerToCamera(GameObject player)
    {
        if (targetGroup != null && player != null)
            targetGroup.AddMember(player.transform, 1f, 2f);
    }
}
