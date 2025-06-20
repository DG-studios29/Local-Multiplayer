﻿using System.Collections.Generic;
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
    public Button continueButton;
    public GameObject confirmButton;

    [Header("Hero Tiles")]
    public List<HeroManager> heroButtons;

    [Header("Player UI")]
    public List<Image> playerHeroImages;
    public List<TextMeshProUGUI> playerHeroNames;
    public RectTransform[] selectors;
    public TextMeshProUGUI[] playerTexts;
    public Image[] playerHeroIcons; 

    private int[] indices = new int[2];
    private bool[] locked = new bool[2];
    private Dictionary<int, string> chosenHeroes = new Dictionary<int, string>();
    private int currentSelectingPlayer = 0;
    private bool p1Selected = false, p2Selected = false;

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < 2; i++)
        {
            indices[i] = 0;
            locked[i] = false;
            UpdateSelector(i);
        }

        chosenHeroes[0] = "";
        chosenHeroes[1] = "";

        continueButton.gameObject.SetActive(false);
        confirmButton.SetActive(true);
        playerUICanvas.SetActive(false);
        mapCanvas.SetActive(false);
    }

    public void Setup(int numberOfPlayers)
    {
        chosenHeroes[0] = "";
        chosenHeroes[1] = "";
        p1Selected = p2Selected = false;

        for (int i = 0; i < numberOfPlayers; i++)
        {
            playerHeroNames[i].text = "Select Hero";
            playerHeroImages[i].sprite = null;
            playerTexts[i].text = $"Player {i + 1}";
            selectors[i].gameObject.SetActive(true);
            locked[i] = false;
        }

        continueButton.gameObject.SetActive(false);

        
    }

    public void OnHeroSelected(string heroName)
    {
        Debug.Log($"Player {currentSelectingPlayer} selected {heroName}");

        if (chosenHeroes.ContainsValue(heroName)) return;

        chosenHeroes[currentSelectingPlayer] = heroName;
        playerHeroNames[currentSelectingPlayer].text = heroName;

        foreach (var button in heroButtons)
        {
            if (button.heroName == heroName)
            {
                playerHeroImages[currentSelectingPlayer].sprite = button.heroPortrait.sprite;
                break;
            }
        }

        confirmButton.SetActive(true);
    }

    public void ConfirmSelection(int playerIndex)
    {
        if (locked[playerIndex]) return;

        int selectedIndex = indices[playerIndex];
        string heroName = heroButtons[selectedIndex].heroName;
        

        chosenHeroes[playerIndex] = heroName;
        SetArrowColorForPlayer(playerIndex);

        Image portrait = heroButtons[selectedIndex].heroPortrait;
        if (playerHeroIcons != null && playerHeroIcons.Length > playerIndex)
        {
            playerHeroIcons[playerIndex].sprite = portrait.sprite;
            playerHeroIcons[playerIndex].enabled = true;
        }

        playerHeroNames[playerIndex].text = heroName;

        foreach (var button in heroButtons)
        {
            if (button.heroName == heroName)
            {
                playerHeroImages[playerIndex].sprite = button.heroPortrait.sprite;
                break;
            }
        }

        locked[playerIndex] = true;
        selectors[playerIndex].gameObject.SetActive(false);
        playerTexts[playerIndex].text = heroName;

        if (playerIndex == 0) p1Selected = true;
        if (playerIndex == 1) p2Selected = true;

        if (p1Selected && p2Selected)
        {
            confirmButton.SetActive(false);
            continueButton.gameObject.SetActive(true);
        }
    }


    public void OnContinuePressed()
    {
        List<string> selectedHeroes = new List<string>();
        for (int i = 0; i < 2; i++)
        {
            if (chosenHeroes.TryGetValue(i, out string hero) && !string.IsNullOrEmpty(hero))
                selectedHeroes.Add(hero);
            else
            {
                Debug.LogWarning($"⚠️ Player {i} has no hero selected. Using Blazeheart.");
                selectedHeroes.Add("Blazeheart");
            }
        }

        selectionPanel.SetActive(false);
        mapCanvas.SetActive(true);
        GameManager.Instance.NotifyHeroSelectionComplete(selectedHeroes);
    }

    public void MoveSelector(int playerIndex, int direction)
    {
        if (locked[playerIndex]) return;

        indices[playerIndex] += direction;

        if (indices[playerIndex] < 0)
            indices[playerIndex] = heroButtons.Count - 1;
        else if (indices[playerIndex] >= heroButtons.Count)
            indices[playerIndex] = 0;

        UpdateSelector(playerIndex);
        currentSelectingPlayer = playerIndex;
    }



    private void UpdateSelector(int playerIndex)
    {
        if (playerIndex < selectors.Length && indices[playerIndex] < heroButtons.Count)
        {
            selectors[playerIndex].position = heroButtons[indices[playerIndex]].transform.position;
        }
    }

    public void CancelPlayerSelection(int playerIndex)
    {
        if (!locked[playerIndex]) return;

        locked[playerIndex] = false;
        chosenHeroes[playerIndex] = "";

        playerHeroNames[playerIndex].text = "Select Hero";
        playerHeroImages[playerIndex].sprite = null;
        playerTexts[playerIndex].text = $"Player {playerIndex + 1}";
        selectors[playerIndex].gameObject.SetActive(true);

        Debug.Log($"🔁 Player {playerIndex} cancelled their selection.");

        // Disable Continue if both aren't locked
        if (!locked[0] || !locked[1])
            continueButton.gameObject.SetActive(false);
    }

    private void SetArrowColorForPlayer(int playerIndex)
    {
        Color arrowColor = Color.white;

        // Define join colors by player index
        switch (playerIndex)
        {
            case 0: arrowColor = Color.red; break;    // Player 1
            case 1: arrowColor = Color.blue; break;   // Player 2
            case 2: arrowColor = Color.yellow; break; // Player 3
            case 3: arrowColor = Color.green; break;  // Player 4
        }

        var players = UnityEngine.Object.FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        foreach (var input in players)
        {
            if (input.playerIndex == playerIndex)
            {
                var arrowImage = input.GetComponentInChildren<UnityEngine.UI.Image>();
                if (arrowImage != null)
                {
                    arrowImage.color = arrowColor;
                    Debug.Log($"🟢 Set arrow color for Player {playerIndex} to {arrowColor}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ Arrow Image not found for Player {playerIndex}");
                }
            }
        }
    }



}
