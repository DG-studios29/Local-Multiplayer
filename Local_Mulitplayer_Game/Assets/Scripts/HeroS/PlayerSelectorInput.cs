using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSelectorInput : MonoBehaviour
{
    private HeroSelectionUI selectionUI;
    private PlayerInput input;
    private int playerIndex;

    private float lastInputTime;
    public float inputCooldown = 0.2f;

    void Awake()
    {
        input = GetComponent<PlayerInput>();
        selectionUI = HeroSelectionUI.Instance;
        playerIndex = input.playerIndex;

        Debug.Log($"[Player {playerIndex}] initialized");
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        Vector2 move = context.ReadValue<Vector2>();
        if (Mathf.Abs(move.x) > 0.5f && Time.time - lastInputTime > inputCooldown)
        {
            int direction = move.x > 0 ? 1 : -1;
            selectionUI?.MoveSelector(playerIndex, direction);
            lastInputTime = Time.time;
            Debug.Log($"Player {playerIndex} MOVE input received from device: {context.control.device}");
        }
    }

    public void OnConfirm(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            selectionUI?.ConfirmSelection(playerIndex);
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            selectionUI?.CancelSelection(playerIndex);
        }
    }

}
