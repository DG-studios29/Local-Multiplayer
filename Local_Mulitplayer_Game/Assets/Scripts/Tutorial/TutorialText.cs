using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "TutorialText", menuName = "Scriptable Objects/TutorialText")]
public class TutorialText : ScriptableObject
{
    [SerializeField][TextArea(4,6)] private string textLine;
    [SerializeField]private bool isToBePerformed;
    [SerializeField]private bool hasVisualInfo;
    [SerializeField]private string actionToPerform;
    //[SerializeField]private InputAction actionMade;
    [SerializeField]private Sprite visualInfoImage;
    
    public string TextLine => textLine;
    public bool IsToBePerformed => isToBePerformed;
    public bool HasVisualInfo => hasVisualInfo;
    public string ActionToPerform => actionToPerform;
    public Sprite VisualInfoImage => visualInfoImage;
    
    //public InputAction ActionMade => actionMade;

}
