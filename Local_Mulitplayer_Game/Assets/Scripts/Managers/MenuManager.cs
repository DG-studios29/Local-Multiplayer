using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
   MainMenu mainMenu;
   public static MenuManager instance;

    private void Start()
    {
        instance = this;
        
    }
    public void PlayScene()
    {

        SceneManager.LoadScene(1);
        
    }

    public void ControlsScene()
    {

        //SceneManager.LoadScene("Controls");

    }

    public void TutorialScene()
    {

        SceneManager.LoadScene(2);

    }


    public void ExitGame()
    {

        Application.Quit();

    }


    public void BackToMenu()
    {
        GameManager.Instance.focusCam.SetActive(false);
        SceneManager.LoadScene("MainMenu");
       // mainMenu.controlsPanel.SetActive(false);

    }


    public void DisplayControlsMenu()

    {

    }


}
