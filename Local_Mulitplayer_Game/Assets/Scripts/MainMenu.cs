using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject controlsPanel;
    public GameObject mainMenuPanel;

    void Awake()
    {
        controlsPanel.SetActive(false);
    }

    // Update is called once per frame
    public void ControlsPanelActivate()

    {
        controlsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);

    }


    public void ControlsPanelDeactivate()

    {

        controlsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}
