using UnityEngine;
using UnityEngine.InputSystem;

public class InputTutorial : MonoBehaviour
{
  
    public void NextTutorial(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TutorialManager.instance.NextTutorialTip();
        }
    }

    public void PreviousTutorial(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            TutorialManager.instance.PreviousTutorialTip();
        }
    }

    
    public void StartGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Go to the gameplay scene with SceneManager
        }
    }
    
    public void PauseTutorial(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Pause Game
            
        }
    }
}
