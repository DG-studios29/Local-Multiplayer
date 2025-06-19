using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class UINavigationManager : MonoBehaviour
{
    public static UINavigationManager Instance;

    public EventSystem eventSystem;
    private GameObject lastSelected;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
       // DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        InputSystem.onEvent += OnInputEvent;
    }

    private void OnDisable()
    {
        InputSystem.onEvent -= OnInputEvent;
    }

    private void Update()
    {
        if (eventSystem.currentSelectedGameObject == null && lastSelected != null)
        {
            eventSystem.SetSelectedGameObject(lastSelected);
        }
        else
        {
            lastSelected = eventSystem.currentSelectedGameObject;
        }
    }

    private void OnInputEvent(InputEventPtr eventPtr, InputDevice device)
    {
        if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
            return;

        if (device is Gamepad)
            EnableGamepadMode();
        else if (device is Keyboard || device is Mouse)
            EnableMouseKeyboardMode();
    }

    private void EnableGamepadMode()
    {
      
        SelectLastOrDefault();
    }

    private void EnableMouseKeyboardMode()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Select(GameObject obj)
    {
        eventSystem.SetSelectedGameObject(obj);
        lastSelected = obj;
    }

    private void SelectLastOrDefault()
    {
        if (eventSystem.currentSelectedGameObject == null)
        {
            var panel = Object.FindAnyObjectByType<UINavigationPanel>();
            if (panel != null && panel.defaultSelected != null)
                Select(panel.defaultSelected);
        }
    }
}
