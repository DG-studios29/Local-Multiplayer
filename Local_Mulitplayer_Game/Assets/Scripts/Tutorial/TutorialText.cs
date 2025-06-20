using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Video;


[CreateAssetMenu(fileName = "TutorialText", menuName = "Scriptable Objects/TutorialText")]
public class TutorialText : ScriptableObject
{
    public enum VisualInfoType
    {NoVisual,Video, Image };
    
    [SerializeField][TextArea(4,6)] private string textLine;
    [SerializeField]private bool isToBePerformed;
    [SerializeField]private bool hasVisualInfo;
    [SerializeField]private string actionToPerform;
    //[SerializeField]private InputAction actionMade;
    [SerializeField]private Sprite visualInfoImage;
    [SerializeField] private VideoClip videoClip;
    [SerializeField] private VisualInfoType visualInfoType;
    
    public string TextLine => textLine;
    public bool IsToBePerformed => isToBePerformed;
    public bool HasVisualInfo => hasVisualInfo;
    public string ActionToPerform => actionToPerform;
    public Sprite VisualInfoImage => visualInfoImage;
    public VideoClip VideoClip => videoClip;
    public VisualInfoType VisualType => visualInfoType;
    //public InputAction ActionMade => actionMade;

}
