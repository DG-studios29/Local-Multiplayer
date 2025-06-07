using UnityEngine;
using UnityEngine.InputSystem;

public class JoinTest : MonoBehaviour
{
    public PlayerJoinUI joinUI;

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            Debug.Log("[TEST] Simulating player join");
            var fakeInput = FindAnyObjectByType<PlayerInput>();
            if (fakeInput != null)
            {
                joinUI.OnPlayerJoined(fakeInput);
            }
            else
            {
                Debug.LogWarning("[TEST] No PlayerInput found in scene.");
            }
        }
    }
}
