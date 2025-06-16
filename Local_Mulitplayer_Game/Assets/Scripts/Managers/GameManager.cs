using System.Collections;
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
    public List<Transform> firehellSpawnPoints;
    public List<Transform> floatingSpawnPoints;
    private List<Transform> currentSpawnPoints;


    [Header("Maps")]
    public string selectedMap = "Forest";
    public GameObject forestMap;
    public GameObject cemeteryMap;
    public GameObject winterMap;
    public GameObject firehellMap;
    public GameObject floatingMap;
    public string CurrentMapName { get; private set; }

    [Header("Camera")]
    public CinemachineTargetGroup targetGroup;

    [Header("Player1 UI")]
    public Slider player1HealthSlider;
    public TMP_Text player1HealthText;
    public TMP_Text player1NameText;


    [Header("Player2 UI")]
    public Slider player2HealthSlider;
    public TMP_Text player2HealthText;
    public TMP_Text player2NameText;


    [Header("Army UI Panels")]
    public MiniArmySpawnerUI player1ArmyUI;
    public MiniArmySpawnerUI player2ArmyUI;

    [Header("P1 Hero Abilities UI")]
    public Image player1Ability1Icon;
    public TMP_Text player1Ability1CooldownText;
    public Image player1Ability2Icon;
    public TMP_Text player1Ability2CooldownText;
    public Image player1Ability3Icon;
    public TMP_Text player1Ability3CooldownText;


    [Header("P2 Hero Abilities UI")]
    public Image player2Ability1Icon;
    public TMP_Text player2Ability1CooldownText;
    public Image player2Ability2Icon;
    public TMP_Text player2Ability2CooldownText;
    public Image player2Ability3Icon;
    public TMP_Text player2Ability3CooldownText;

    [Header("Army Kill UI")]
    public TMP_Text player1KillsText;
    public TMP_Text player2KillsText;

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
        else
        {
            if (gameStarted)
            {
                gameStarted = false;
                Debug.Log("Time's up! Calling DetermineWinner...");
                DetermineWinner();
            }
        }
    }

    public void UpdateKillUI()
    {
        if (activePlayers.Count < 2) return;

        var p1 = activePlayers.Find(p => p.name == "Player 1");
        var p2 = activePlayers.Find(p => p.name == "Player 2");

        if (p1 != null && p2 != null)
        {
            var p1Stats = p1.GetComponent<PlayerStats>();
            var p2Stats = p2.GetComponent<PlayerStats>();

            player1KillsText.text = $"Kills: {p1Stats.armyKills}";
            player2KillsText.text = $"Kills: {p2Stats.armyKills}";
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
        StartCoroutine(SpawnPlayersDelayed());
    }

    public void SelectMap(string mapName)
    {
        forestMap.SetActive(false);
        cemeteryMap.SetActive(false);
        winterMap.SetActive(false);

        switch (selectedMap.ToLower())
        {
            case "forest":
                currentSpawnPoints = forestSpawnPoints;
                forestMap.SetActive(true);
                break;
            case "cemetery":
                currentSpawnPoints = cemeterySpawnPoints;
                cemeteryMap.SetActive(true);
                break;
            case "winter":
                currentSpawnPoints = winterSpawnPoints;
                winterMap.SetActive(true);
                break;
            case "firehell":
                currentSpawnPoints = firehellSpawnPoints;
                firehellMap.SetActive(true);
                break;
            case "floating":
                currentSpawnPoints = floatingSpawnPoints;
                floatingMap.SetActive(true);
                break;
            default:
                Debug.LogError("Invalid map name: " + selectedMap);
                break;
        }



        var portalSpawner = Object.FindAnyObjectByType<PortalSpawner>();
        if (portalSpawner != null)
            portalSpawner.SetCurrentMap(mapName);

        CurrentMapName = mapName;
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

    public void DetermineWinner()
    {
        GameObject p1 = activePlayers.Find(p => p != null && p.name == "Player 1");
        GameObject p2 = activePlayers.Find(p => p != null && p.name == "Player 2");

        bool p1Alive = p1 != null && p1.GetComponent<PlayerHealth>().IsAlive;
        bool p2Alive = p2 != null && p2.GetComponent<PlayerHealth>().IsAlive;

        string result;

        if (p1Alive && !p2Alive)
            result = "🏆 Player 1 wins by elimination!";
        else if (!p1Alive && p2Alive)
            result = "🏆 Player 2 wins by elimination!";
        else
        {
            int p1Kills = p1 != null ? p1.GetComponent<PlayerStats>().GetArmyKills() : 0;
            int p2Kills = p2 != null ? p2.GetComponent<PlayerStats>().GetArmyKills() : 0;

            if (p1Kills > p2Kills)
                result = "🏆 Player 1 wins by army kills!";
            else if (p2Kills > p1Kills)
                result = "🏆 Player 2 wins by army kills!";
            else
                result = "🤝 It's a draw!";
        }

        Debug.Log(result);
        GameOverManager.ShowResult(result);
    }



    public void OnPlayerDeath(GameObject deadPlayer)
    {
        if (deadPlayer == null)
        {
            Debug.LogWarning("⚠️ OnPlayerDeath called with null player.");
            return;
        }

        Debug.Log($"☠️ {deadPlayer.name} has died.");
        DetermineWinner(); // Call safe version
    }

    private IEnumerator SpawnPlayersDelayed()
    {
        yield return null;

        var players = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        for (int i = 0; i < players.Length; i++)
        {
            PlayerInput input = players[i];
            GameObject player = input.gameObject;
            int index = input.playerIndex;

            Vector3 spawnPos = currentSpawnPoints.Count > index
                ? currentSpawnPoints[index].position
                : Vector3.zero;

            // Raycast check (optional safety)
            if (!Physics.Raycast(spawnPos + Vector3.up * 2f, Vector3.down, out RaycastHit hit, 5f))
            {
                Debug.LogWarning($"No ground detected under spawn point {index}. Defaulting to 0,1,0.");
                spawnPos = new Vector3(0f, 1f, 0f);
            }

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
            player.transform.position = spawnPos + Vector3.up * 1f;
            player.transform.rotation = currentSpawnPoints[index].rotation;
            player.name = $"Player {index + 1}";

            RegisterPlayer(input);
            AssignHeroScript(player, selectedHeroes[index]);
            AssignPlayerMaterials(player, index);
            SetupPlayerUI(player, player.name);
            SetupHeroAbilitiesUI(player);

            if (player.GetComponent<PlayerStats>() == null)
                player.AddComponent<PlayerStats>();
        }

        HeroSelectionUI.Instance.playerUICanvas.SetActive(true);
    }



}
