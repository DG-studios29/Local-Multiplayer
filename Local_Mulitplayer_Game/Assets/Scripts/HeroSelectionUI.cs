using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class HeroSelectionUI : MonoBehaviour
{
    public static HeroSelectionUI Instance;

    [Header("UI References")]
    public GameObject selectionPanel;
    public GameObject playerUICanvas;
    public GameObject mapCanvas; 
    public Button startGameButton;

    [Header("Hero Setup")]
    public List<HeroManager> heroButtons;
    public List<Image> playerHeroImages;
    public List<TextMeshProUGUI> playerHeroNames;

    [Header("Hero Slots (Left to Right)")]
    public RectTransform[] heroSlots;

    [Header("Player UI")]
    public RectTransform[] selectors; // P1 = 0, P2 = 1
    public TextMeshProUGUI[] playerTexts;
    public GameObject confirmButton;

    private int[] indices = new int[2];
    private bool[] locked = new bool[2];

    private Dictionary<int, string> chosenHeroes = new Dictionary<int, string>();
    private int currentSelectingPlayer = 0;

    bool p1Selected, p2Selected;

    private void Awake()
    {
        for (int i = 0; i < 2; i++)
        {
            indices[i] = 0;
            UpdateSelector(i);
        }
        confirmButton.SetActive(false);
        Instance = this;
        playerUICanvas.SetActive(false);
        mapCanvas.SetActive(false); 
    }
   
    public void Setup(int numberOfPlayers)
    {
        //selectionPanel.SetActive(true);
        chosenHeroes.Clear();
        currentSelectingPlayer = 0;

        for (int i = 0; i < numberOfPlayers; i++)
        {
            chosenHeroes[i] = "";
            playerHeroImages[i].sprite = null;
            playerHeroNames[i].text = "Select Hero";
        }

        startGameButton.interactable = false;

        foreach (var button in heroButtons)
        {
            button.Initialize();
        }
    }

    public void OnHeroSelected(string heroName)
    {
        if (chosenHeroes.ContainsValue(heroName))
            return;

        if (chosenHeroes.ContainsKey(currentSelectingPlayer))
        {
            chosenHeroes[currentSelectingPlayer] = heroName;
            playerHeroNames[currentSelectingPlayer].text = heroName;

            // Example: you could set an icon here too
            // playerHeroImages[currentSelectingPlayer].sprite = HeroDatabase.GetSprite(heroName);

            currentSelectingPlayer++;

            if (currentSelectingPlayer >= chosenHeroes.Count)
            {
                startGameButton.interactable = true;
                GameManager.Instance.selectedHeroes = new List<string>(chosenHeroes.Values);
            }
        }
    }

    public void PlayerSelected(int playerIndex)
    {
        if (playerIndex == 0) p1Selected = true;
        if (playerIndex == 1) p2Selected = true;

        startGameButton.interactable = (p1Selected && p2Selected);
    }

    public void OnStartGamePressed()
    {
        selectionPanel.SetActive(false);
        mapCanvas.SetActive(true); 
    }

    public void MoveSelector(int playerIndex, int direction)
    {
        if (locked[playerIndex]) return;

        indices[playerIndex] += direction;
        indices[playerIndex] = Mathf.Clamp(indices[playerIndex], 0, heroSlots.Length - 1);
        UpdateSelector(playerIndex);
    }

    public void ConfirmSelection(int playerIndex)
    {
        if (locked[playerIndex]) return;

        HeroTileUI tile = heroSlots[indices[playerIndex]].GetComponent<HeroTileUI>();
        if (tile != null)
        {
            locked[playerIndex] = true;
            playerTexts[playerIndex].text = tile.heroName;

            // Set name + (optional) icon
            playerHeroNames[playerIndex].text = tile.heroName;
            if (tile.heroIcon != null && playerHeroImages.Count > playerIndex)
                playerHeroImages[playerIndex].sprite = tile.heroIcon;

            // Mark the hero as selected
            chosenHeroes[playerIndex] = tile.heroName;

            // Optionally: switch to the next player if you want sequential selection
            currentSelectingPlayer++;

            // Call this to mark the player as confirmed
            PlayerSelected(playerIndex);
        }

        // Check if both players are done
        if (locked[0] && locked[1])
        {
            confirmButton.SetActive(true);
            startGameButton.interactable = true;
            GameManager.Instance.selectedHeroes = new List<string>(chosenHeroes.Values);
        }
    }
    public void CancelSelection(int playerIndex)
    {
        if (!locked[playerIndex]) return;

        locked[playerIndex] = false;
        playerTexts[playerIndex].text = "Selecting...";

        if (playerHeroNames.Count > playerIndex)
            playerHeroNames[playerIndex].text = "Select Hero";

        if (playerHeroImages.Count > playerIndex)
            playerHeroImages[playerIndex].sprite = null;

        if (chosenHeroes.ContainsKey(playerIndex))
            chosenHeroes[playerIndex] = "";

        // Reset player confirmed state
        if (playerIndex == 0) p1Selected = false;
        if (playerIndex == 1) p2Selected = false;

        // Disable confirm/start until both reselect
        confirmButton.SetActive(false);
        startGameButton.interactable = false;

        // Optional: reposition selector back to index 0
        indices[playerIndex] = 0;
        UpdateSelector(playerIndex);
    }

    private void UpdateSelector(int playerIndex)
    {
        selectors[playerIndex].position = heroSlots[indices[playerIndex]].position;
    }
}
