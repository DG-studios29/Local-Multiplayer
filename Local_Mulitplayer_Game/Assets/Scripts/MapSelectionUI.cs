using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapSelectionUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject mapCanvas;

    private List<string> availableMaps = new List<string>
    {
        "forest", "cemetery", "winter", "floating", "firehell"
    };

    public void SelectMap(string mapName)
    {
        GameManager.Instance.selectedMap = mapName.ToLower();
    }

    public void SelectRandomMap()
    {
        int randomIndex = Random.Range(0, availableMaps.Count);
        string randomMap = availableMaps[randomIndex];
        GameManager.Instance.selectedMap = randomMap;
        Debug.Log("Random map selected: " + randomMap);
    }

    public void OnConfirmMap()
    {
        List<string> chosenHeroes = GameManager.Instance.selectedHeroes;

        if (string.IsNullOrEmpty(GameManager.Instance.selectedMap))
        {
            Debug.LogWarning("No map selected! Please select a map first.");
            return;
        }

        if (chosenHeroes == null || chosenHeroes.Count == 0)
        {
            Debug.LogWarning("No heroes selected! Cannot start game.");
            return;
        }

        foreach (var player in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
        {
            player.SwitchCurrentActionMap("Player");
        }

        this.gameObject.SetActive(false);
        mapCanvas.SetActive(false);
        GameManager.Instance.StartGame(chosenHeroes);
        ArenaEventManager.Instance.StartEventRoutine();
    }
}
