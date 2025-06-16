using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Utilities;
using TMPro;
using static UnityEngine.Rendering.DebugUI;
using System.Collections;

public class PlayerJoinManager : MonoBehaviour
{
    public PlayerInputManager playerInputManager;
    public GameObject joinPanel; 
    public GameObject heroPanel; 
    public GameObject continueButton;
    public GameObject noEnoughPlayers;
    private PlayerControls inputActions;
    private InputAction joinAction;
    private int joinedPlayers = 0;
    public float showDuration = 4f;
    private bool blockNextSubmit;

    private void Awake()
    {
        inputActions = new PlayerControls();
        joinAction = inputActions.Player.PlayerJoin;
        noEnoughPlayers.SetActive(false);
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

    void Update()
    {
        if (blockNextSubmit && joinAction.triggered)
        {
            blockNextSubmit = false;
            return;
        }
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
            noEnoughPlayers.SetActive(true);
            Debug.LogWarning("[JoinManager] Not enough players to continue.");
            StartCoroutine(HideAfterDelay());
            return;
        }

        

        joinPanel.SetActive(false);
        heroPanel.SetActive(true);
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(showDuration);
        noEnoughPlayers.SetActive(false);
    }
}
