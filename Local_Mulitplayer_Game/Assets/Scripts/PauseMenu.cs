using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    private PlayerInput playerInput;
    public GameObject controlsPanel;
    bool isPaused = false;

  

    private void Start()
    {
        if (playerInput != null)
        {
            playerInput.actions["Pause"].performed += ctx => GamePaused();
        }
        

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {



            GamePaused();

        }

       

    }

    public void ControlsPanelActivate()

    {
        controlsPanel.SetActive(true);
        pausePanel.SetActive(false);

    }

    public void ControlsPanelDeactivate()

    {

        controlsPanel.SetActive(false);
        pausePanel.SetActive(true);
    }

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    public void GamePaused ()
    {

        pausePanel.SetActive(true);
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);        
        }

    }


    public void BackToGame()
    {

        pausePanel.SetActive(false);

    }


}
