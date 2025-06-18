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
        if (string.IsNullOrEmpty(GameManager.Instance.selectedMap))
        {
            Debug.LogWarning("No map selected! Please select a map first.");
            return;
        }

        foreach (var player in FindObjectsByType<PlayerInput>(FindObjectsSortMode.None))
        {
            player.SwitchCurrentActionMap("Player");
        }


        mapCanvas.SetActive(false);

  
        GameManager.Instance.NotifyMapSelected(GameManager.Instance.selectedMap);

        ArenaEventManager.Instance.StartEventRoutine();
    }

}
