using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroSelectionUI : MonoBehaviour
{
    public static HeroSelectionUI Instance;

    public GameObject selectionPanel;
    public GameObject playerUICanvas;
    public Button startGameButton;
    public List<HeroManager> heroButtons;

    public List<Image> playerHeroImages;
    public List<TextMeshProUGUI> playerHeroNames;

    private Dictionary<int, string> chosenHeroes = new Dictionary<int, string>();
    private int currentSelectingPlayer = 0;

    private void Awake()
    {
        Instance = this;
        playerUICanvas.SetActive(false);
    }

    public void Setup(int numberOfPlayers)
    {
        selectionPanel.SetActive(true);
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
        // Prevent duplicate hero assignment
        if (chosenHeroes.ContainsValue(heroName))
        {
            
            return;
        }

        // Assign hero to the current player
        if (chosenHeroes.ContainsKey(currentSelectingPlayer))
        {
            chosenHeroes[currentSelectingPlayer] = heroName;
            Debug.Log($"Player {currentSelectingPlayer + 1} chosen hero: {heroName}");

            UpdatePlayerHeroUI(currentSelectingPlayer, heroName);

            // Only advance if the player hasn't already picked
            currentSelectingPlayer++;
            if (currentSelectingPlayer >= chosenHeroes.Count)
                currentSelectingPlayer = chosenHeroes.Count - 1; // Clamp
        }

        // Confirm button active only when all slots are filled
        startGameButton.interactable = AreAllHeroesSelected();
    }



    private void UpdatePlayerHeroUI(int playerIndex, string heroName)
    {
        Sprite heroSprite = GetHeroSprite(heroName);
        playerHeroImages[playerIndex].sprite = heroSprite;
        playerHeroNames[playerIndex].text = heroName;
    }

    private Sprite GetHeroSprite(string heroName)
    {
        return Resources.Load<Sprite>($"Heroes/{heroName}");
    }

    public void OnHeroChange()
    {
        if (chosenHeroes.ContainsKey(currentSelectingPlayer))
        {
            chosenHeroes[currentSelectingPlayer] = "";
            Debug.Log($"Player {currentSelectingPlayer + 1} changed their selection.");
        }
    }

    private bool AreAllHeroesSelected()
    {
        foreach (var hero in chosenHeroes.Values)
        {
            if (string.IsNullOrEmpty(hero)) return false;
        }
        return true;
    }

    public void OnStartGame()
    {
        selectionPanel.SetActive(false);
        playerUICanvas.SetActive(true);

        GameManager.Instance.selectedHeroes = new List<string>();

        foreach (var kvp in chosenHeroes)
        {
            Debug.Log($"Player {kvp.Key} picked hero: {kvp.Value}");
            GameManager.Instance.selectedHeroes.Add(kvp.Value);
        }

        GameManager.Instance.StartGame(GameManager.Instance.selectedHeroes);
    }
}
