using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
   MainMenu mainMenu;

    private void Start()
    {
        
    }
    public void PlayScene()
    {
        
        SceneManager.LoadScene("GamePlay");
        
    }

    public void ControlsScene()
    {

        //SceneManager.LoadScene("Controls");

    }


    public void ExitGame()
    {

        Application.Quit();

    }


    public void BackToMenu()
    {

        SceneManager.LoadScene("MainMenu");
       // mainMenu.controlsPanel.SetActive(false);

    }


    public void DisplayControlsMenu()

    {

    }


}
