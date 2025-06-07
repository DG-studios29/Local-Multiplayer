using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;

public class PlayerJoinManager : MonoBehaviour
{
    public PlayerInputManager playerInputManager;
    public GameObject joinPanel; 
    public GameObject heroPanel; 
    public GameObject continueButton; 
    private PlayerControls inputActions;
    private InputAction joinAction;
    private int joinedPlayers = 0;

    private void Awake()
    {
        inputActions = new PlayerControls();
        joinAction = inputActions.Player.PlayerJoin;
    }

    private void OnEnable()
    {
        joinAction.Enable();
        joinAction.performed += OnJoinGame;
    }

    private void OnDisable()
    {
        joinAction.Disable();
        joinAction.performed -= OnJoinGame;
    }

    private void OnJoinGame(InputAction.CallbackContext context)
    {
        joinedPlayers++;
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.JoinPlayerFromActionIfNotAlreadyJoined(context);
        }
        else
        {
            Debug.LogWarning("[JoinManager] PlayerInputManager instance is null.");
        }

        if (joinedPlayers >= 2)
        {
            continueButton.SetActive(true); // enable the button
        }
    }

    public void ContinueToHeroSelect()
    {
        if (joinedPlayers < 2)
        {
            Debug.LogWarning("[JoinManager] Not enough players to continue.");
            return;
        }

        Debug.Log("[JoinManager] Transitioning to Hero Selection...");

        joinPanel.SetActive(false);
        heroPanel.SetActive(true);
    }
}
