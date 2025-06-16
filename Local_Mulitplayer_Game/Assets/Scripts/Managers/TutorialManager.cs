using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    public bool isTutorialActive;
    
    [Header("Tutorial Input Tracking")]
    //public List<GameObject> activePlayers = new List<GameObject>();
    [SerializeField]private List<PlayerController> playersInputs = new List<PlayerController>();
    
    [SerializeField]private List<TutorialText> tutorialTexts = new List<TutorialText>();

    [SerializeField]private int tipCounter;
    private TutorialText currentTip; //keep track of what we are tracking
    private bool canGetNextTip = false;

    [Header("Tutorial Text Display")] 
    [SerializeField] private GameObject tutorialTextButtons;
    [SerializeField] private TMP_Text tipCounterText;
    [SerializeField] private GameObject visualInfoHolder;
    [SerializeField] private Image visualInfoImage;
    [SerializeField] private TMP_Text successText;
    [SerializeField] private TMP_Text tutorialTipText;
   


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Access all our player inputs
        playersInputs.AddRange(GameObject.FindObjectsByType<PlayerController>(FindObjectsSortMode.None));

        tipCounter = 0;
        
        successText.enabled = false;
        visualInfoHolder.SetActive(false);
        tutorialTextButtons.SetActive(false);
        
        ShowTutorialTip();
        
    }

    // Update is called once per frame
    /*void Update()
    {
        
    }*/

    void SwitchToTutorialMap()
    {
        foreach (PlayerController player in playersInputs)
        {
            player.SwitchInputTutorial();
        }
    }

    void SwitchToPlayerMap()
    {
        foreach (PlayerController player in playersInputs)
        {
            player.SwitchInputTutorial();
        }
    }

    void SwitchToUIMap()
    {
        foreach (PlayerController player in playersInputs)
        {
            player.SwitchInputUI();
        }
    }

    public void NextTutorialTip(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            tipCounter++;
            ShowTutorialTip();
        }
    }

    public void PreviousTutorialTip(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            tipCounter--;
            ShowTutorialTip();
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

    private void ShowTutorialTip()
    {
        //tipCounter++;
        currentTip = tutorialTexts[tipCounter];
        
        //Deactivate Prev and Next Buttons
        ToggleTutorialButtons();
        
        //Update Visuals
        tutorialTipText.text = currentTip.TextLine;

        if (currentTip.HasVisualInfo)
        {
            //Do what needs to be shown
            canGetNextTip = true;
            visualInfoHolder.SetActive(true);
            visualInfoImage.sprite = currentTip.VisualInfoImage.sprite;
            
            //Activate Prev and Next Buttons
            ToggleTutorialButtons();    
        }

        //Initialize a condition
        if (currentTip.IsToBePerformed)
        {
            canGetNextTip = false;
        }
        else
        {
            canGetNextTip = true;
            
            //Activate Prev and Next Buttons
            ToggleTutorialButtons();
        }
        
    }

    public void CheckTutorialPerform(string actionName)
    {
        if (currentTip.IsToBePerformed)
        {
            //Check Match
            if (currentTip.ActionToPerform == actionName)
            {
                //Success has been met, and we can now progress 
                canGetNextTip = true;
                
                //Activate Prev and Next Buttons
                ToggleTutorialButtons();
            }
            
        }
    }

    private void ToggleTutorialButtons()
    {
        if (tutorialTextButtons.activeSelf)
        {
            SwitchToPlayerMap();
            tutorialTextButtons.SetActive(false);
        }
        else
        {
            SwitchToTutorialMap();
            
            successText.enabled = currentTip.IsToBePerformed;
            
            tutorialTextButtons.SetActive(true);
        }
        
    }

    private void ToggleOffPrevious()
    {
        SwitchToPlayerMap();
        tutorialTextButtons.SetActive(false);
        visualInfoHolder.SetActive(false);
    }    
}
