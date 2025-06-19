using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerPromptUI : MonoBehaviour
{
    public Image Ability1Icon;
    public Image Ability2Icon;
    public Image Ability3Icon;
    public Image TankIcon;
    public Image WarriorIcon;
    public Image HealerIcon;
    public Image MiniIcon;


    public PromptSet keyboardPrompts;
    public PromptSet controllerPrompts;

    /// <summary>Call this right after a player joins or when their control-scheme changes.</summary>
    public void SetupPrompts(PlayerInput playerInput)
    {
        if (playerInput == null)
        {
            Debug.LogError("[PromptUI] playerInput was NULL - cannot set prompts");
            return;
        }

        // Choose the correct prompt set
        bool usingGamepad = playerInput.currentControlScheme.Contains("Gamepad");
        PromptSet promptSet = usingGamepad ? controllerPrompts : keyboardPrompts;

        Debug.Log($"[PromptUI] Player {playerInput.playerIndex} " +
                  $"ControlScheme = {playerInput.currentControlScheme} " +
                  $"→ Using {(usingGamepad ? "Controller" : "Keyboard")} PromptSet ©{promptSet?.name}");

        // Defensive null checks per sprite
        SetIcon(Ability1Icon, promptSet?.Ability1, "Ability1");
        SetIcon(Ability2Icon, promptSet?.Ability2, "Ability2");
        SetIcon(Ability3Icon, promptSet?.Ability3, "Ability3");
        SetIcon(TankIcon, promptSet?.Tank, "Tank");
        SetIcon(WarriorIcon, promptSet?.Warrior, "Warrior");
        SetIcon(HealerIcon, promptSet?.Healer, "Healer");
        SetIcon(MiniIcon, promptSet?.Mini, "Mini");
    }

    private void SetIcon(Image img, Sprite sprite, string label)
    {
        if (img == null)
        {
            Debug.LogWarning($"[PromptUI] Image ref for {label} is missing on {name}");
            return;
        }
        if (sprite == null)
        {
            Debug.LogWarning($"[PromptUI] {label} sprite is NULL in PromptSet on {name}");
        }

        img.sprite = sprite;
    }
}
