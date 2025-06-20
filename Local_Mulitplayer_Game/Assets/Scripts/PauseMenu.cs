using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using static UnityEngine.Timeline.DirectorControlPlayable;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    private PlayerInput playerInput;
    public GameObject controlsPanel;

    public GameObject playerUIPanel;
    bool isPaused = false;
    
    public static PauseMenu Instance { get; private set; }

    private void Start()
    {
        isPaused = false;
        Time.timeScale = 1f;

        // if (playerInput != null)
        // {
        //     playerInput.actions["Pause"].performed += ctx => GamePaused();
        // }

    }
    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Escape))
        // {



        //     GamePaused();

        // }

       
        // if (playerInput != null && playerInput.actions["Pause"].triggered)
        // {
        //     GamePaused();
        // }



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
        if (playerInput != null)
        {
            playerInput.actions["Pause"].performed += ctx => GamePaused();
        }

        Instance = this;

    }

    public void GamePaused()
    {

        pausePanel.SetActive(true);
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
            playerUIPanel.SetActive(!isPaused);
        }

    }


    public void BackToGame()
    {


        // pausePanel.SetActive(isPaused);
        // playerUIPanel.SetActive(true);
        // Time.timeScale = 1;

         pausePanel.SetActive(false);
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;

        if (pausePanel != null)
        {
            pausePanel.SetActive(isPaused);
            playerUIPanel.SetActive(!isPaused);
        }

    }
    
    public void BackToMainMenu()
    {

        SceneManager.LoadScene("MainMenu");

    }


}
