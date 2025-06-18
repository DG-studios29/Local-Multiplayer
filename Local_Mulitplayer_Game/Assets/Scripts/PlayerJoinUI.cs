using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PlayerJoinUI : MonoBehaviour
{
    public GameObject[] playerPanels;
    public TextMeshProUGUI[] playerLabels;
    public Color[] playerColors;
    public Image[] playerInputIcons;
    public Sprite keyboardRedIcon;
    public Sprite controllerRedIcon;
    public Sprite keyboardBlueIcon;
    public Sprite controllerBlueIcon;

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
            foreach (var img in playerPanels[index].GetComponentsInChildren<Image>())
                img.color = new Color(playerColors[index].r, playerColors[index].g, playerColors[index].b, img.color.a);
        }

        if (playerInputIcons.Length > index && playerInputIcons[index] != null)
        {
            bool isKeyboard = playerInput.currentControlScheme == "Keyboard&Mouse" || playerInput.currentControlScheme == "Keyboard";

            if (index == 0) // Player 1 = Red
            {
                playerInputIcons[index].sprite = isKeyboard ? keyboardRedIcon : controllerRedIcon;
            }
            else if (index == 1) // Player 2 = Blue
            {
                playerInputIcons[index].sprite = isKeyboard ? keyboardBlueIcon : controllerBlueIcon;
            }
        }


        GameManager.Instance?.RegisterPlayer(playerInput);
    }
}
