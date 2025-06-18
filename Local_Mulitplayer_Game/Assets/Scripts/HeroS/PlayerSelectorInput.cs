using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSelectorInput : MonoBehaviour
{
    private HeroSelectionUI selectionUI;
    private PlayerInput input;
    private int playerIndex;

    private float lastInputTime;
    public float inputCooldown = 0.2f;
    private float scrollCooldown = 0.2f;
    private float lastScrollTime = -1f;

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

    public void OnNavigate(InputAction.CallbackContext context)
    {
        Vector2 nav = context.ReadValue<Vector2>();

        if (context.performed && Time.time - lastScrollTime > scrollCooldown)
        {
            if (Mathf.Abs(nav.x) > 0.5f)
            {
                int direction = nav.x > 0 ? 1 : -1;
                HeroSelectionUI.Instance.MoveSelector(playerIndex, direction);
                lastScrollTime = Time.time;
            }
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
            HeroSelectionUI.Instance.CancelPlayerSelection(playerIndex);
        }
    }
}
