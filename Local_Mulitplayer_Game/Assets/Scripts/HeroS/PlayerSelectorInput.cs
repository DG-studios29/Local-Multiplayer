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

        if (selectionUI == null)
            Debug.LogError($"❌ Player {playerIndex} couldn't find HeroSelectionUI!");
        else
            Debug.Log($"✅ Player {playerIndex} initialized input for selection.");
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
