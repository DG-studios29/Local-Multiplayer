// PlayerJoinUI.cs
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerJoinUI : MonoBehaviour
{
    public GameObject[] playerPanels;
    public TextMeshProUGUI[] playerLabels;
    public Color[] playerColors;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        int index = playerInput.playerIndex;

        if (index < 0 || index >= playerPanels.Length)
        {
            Debug.LogWarning($"[UI] Invalid player index: {index}");
            return;
        }

        playerPanels[index].SetActive(true);
        if (playerLabels.Length > index && playerLabels[index] != null)
            playerLabels[index].text = $"Player {index + 1} Joined!";

        if (playerColors.Length > index)
        {
            foreach (var img in playerPanels[index].GetComponentsInChildren<UnityEngine.UI.Image>())
                img.color = new Color(playerColors[index].r, playerColors[index].g, playerColors[index].b, img.color.a);
        }

        GameManager.Instance?.RegisterPlayer(playerInput);
    }
}
