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
    public List<GameObject> activePlayers = new List<GameObject>();
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
    [SerializeField] private GameObject tutorialFilter;

    //[SerializeField] private GameObject tutorialTextPanel;

    private void Awake()
    {
        if (instance && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
    }
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    

    // Update is called once per frame
    /*void Update()
    {
        
    }*/
    public void TutorialStarted()
    {
        Debug.Log("Tutorial Started");
        
        //Access all our player inputs
        playersInputs.AddRange(GameObject.FindObjectsByType<PlayerController>(FindObjectsSortMode.None));

        tipCounter = 0;
        
        successText.enabled = false;
        visualInfoHolder.SetActive(false);
        tutorialTextButtons.SetActive(false);
        tutorialFilter.SetActive(false);
        
        ShowTutorialTip();
    }
    

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
            player.SwitchInputPlayer();
        }
    }

    void SwitchToUIMap()
    {
        foreach (PlayerController player in playersInputs)
        {
            player.SwitchInputUI();
        }
    }
    
    
    
    public void NextTutorialTip()
    {
         if (tipCounter >= tutorialTexts.Count - 1)
         {
             tipCounter = tutorialTexts.Count - 1;
         }
         else
         {
             tipCounter++;
             ShowTutorialTip();
         }
         
         
        
    }

    public void PreviousTutorialTip()
    {
        
        if (tipCounter <= 0)
        {
            tipCounter = 0;
        }
        else
        {
            tipCounter--;
            ShowTutorialTip();
        }
        
    }
    
   

    private void ShowTutorialTip()
    {
        //Deactivate Prev and Next Buttons
        ToggleOffPrevious();
        
        var properCount = tipCounter + 1;
        tipCounterText.text = properCount.ToString() + "/" + tutorialTexts.Count;
        
        currentTip = tutorialTexts[tipCounter];
        
        //Update Text
        tutorialTipText.text = currentTip.TextLine;

        if (currentTip.HasVisualInfo)
        {
            //Do what needs to be shown
            canGetNextTip = true;
            visualInfoHolder.SetActive(true);
            visualInfoImage.sprite = currentTip.VisualInfoImage;
            
            //Activate Prev and Next Buttons
            ToggleTutorialButtons();    
        }

        //Initialize a condition
        if (currentTip.IsToBePerformed)
        {
            //needs to move around for a bit until
            SwitchToPlayerMap();
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
        if(!currentTip) return;
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
        if (!tutorialTextButtons.activeSelf)
        {
            SwitchToTutorialMap();
            
            successText.enabled = currentTip.IsToBePerformed;
            
            tutorialTextButtons.SetActive(true);
            tutorialFilter.SetActive(true);
        }
        
    }

    private void ToggleOffPrevious()
    {
        //SwitchToPlayerMap();
        tutorialFilter.SetActive(false);
        tutorialTextButtons.SetActive(false);
        visualInfoHolder.SetActive(false);
    }    
}
