using UnityEngine;

public class UINavigationPanel : MonoBehaviour
{
    public GameObject defaultSelected;

    private void OnEnable()
    {
        if (UINavigationManager.Instance != null && defaultSelected != null)
        {
            UINavigationManager.Instance.Select(defaultSelected);
        }
    }
}
