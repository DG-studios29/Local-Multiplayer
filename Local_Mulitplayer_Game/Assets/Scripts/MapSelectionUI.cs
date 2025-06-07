using UnityEngine;
using UnityEngine.UI;

public class MapSelectionUI : MonoBehaviour
{
    public GameObject mapSelectionPanel;
    public Button forestButton;
    public Button cemeteryButton;
    public Button winterButton;
    public Button confirmButton;

    private string selectedMap = "Forest"; // Default selection

    private void Start()
    {
        forestButton.onClick.AddListener(() => SelectMap("Forest"));
        cemeteryButton.onClick.AddListener(() => SelectMap("Cemetery"));
        winterButton.onClick.AddListener(() => SelectMap("Winter"));
        confirmButton.onClick.AddListener(ConfirmSelection);
    }

    void SelectMap(string mapName)
    {
        selectedMap = mapName;
        Debug.Log($"Selected Map: {mapName}");
    }

    public void ConfirmSelection()
    {
        GameManager.Instance.SelectMap(selectedMap);
        mapSelectionPanel.SetActive(false);
        HeroSelectionUI.Instance.selectionPanel.SetActive(true);
    }
}
