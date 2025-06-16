using UnityEngine;
using UnityEngine.InputSystem;


[CreateAssetMenu(fileName = "TutorialText", menuName = "Scriptable Objects/TutorialText")]
public class TutorialText : ScriptableObject
{
    [TextArea(4,6)] private string textLine;
    private bool isToBePerformed;
    private bool hasVisualInfo;
    private string actionToPerform;
    private InputAction actionMade;
    
    public string TextLine => textLine;
    public bool IsToBePerformed => isToBePerformed;
    public bool HasVisualInfo => hasVisualInfo;
    public string ActionToPerform => actionToPerform;
    
    public InputAction ActionMade => actionMade;

}
