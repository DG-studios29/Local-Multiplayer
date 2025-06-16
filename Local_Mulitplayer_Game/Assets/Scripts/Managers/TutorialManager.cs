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

    [Header("Tutorial Text Display")] 
    [SerializeField] private GameObject tutorialTextButtons;
    [SerializeField] private Image visualInfoImage;
    [SerializeField] private TMP_Text successText;
    [SerializeField] private TMP_Text tutorialTip;
   


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Access all our player inputs
        playersInputs.AddRange(GameObject.FindObjectsByType<PlayerController>(FindObjectsSortMode.None));

        tipCounter = 0;
        
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
            
        }
    }

    public void PreviousTutorialTip(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            
        }
    }

    public void StartGame(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Go to the gameplay scene
        }
    }
    
    public void PauseTutorial(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //Pause Game
        }
    }

    public void ShowTutorialTip()
    {
        tipCounter++;
        currentTip = tutorialTexts[tipCounter];

        if (currentTip.HasVisualInfo)
        {
            //Do what needs to be shown
        }

        if (currentTip.IsToBePerformed)
        {
            //Initialize a condition
            
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
                
            }
            
        }
    }
    
}
