using UnityEngine;

public class MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject controlsPanel;
    public GameObject mainMenuPanel;

    public GameObject storyMenuPanel;

    void Awake()
    {
        controlsPanel.SetActive(false);
    }

    // Update is called once per frame
    public void ControlsPanelActivate()

    {
        controlsPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        storyMenuPanel.SetActive(false);

    }


    public void ControlsPanelDeactivate()

    {

        controlsPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void StoryMenuActivate()

    {
        storyMenuPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        controlsPanel.SetActive(false);
    }

    public void StoryMenuDeactivate()

    {
        storyMenuPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        controlsPanel.SetActive(false);
    }
}
