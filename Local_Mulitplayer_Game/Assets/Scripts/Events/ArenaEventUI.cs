using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ArenaEventUI : MonoBehaviour
{
    public GameObject panel;
    public TMP_Text eventTitle;
    public TMP_Text eventDescription;
    public Image eventIcon;

    public float showDuration = 4f;

    private void OnEnable()
    {
        ArenaEventManager.OnArenaEventStart += DisplayEvent;
        ArenaEventManager.OnArenaEventEnd += DisplayEventEnd;
    }

    private void OnDisable()
    {
        ArenaEventManager.OnArenaEventStart -= DisplayEvent;
        ArenaEventManager.OnArenaEventEnd -= DisplayEventEnd;
    }

    private void DisplayEvent(ArenaEventSO evt)
    {
        eventTitle.text = evt.eventName;
        eventDescription.text = evt.description;

        if (eventIcon != null && evt.icon != null)
            eventIcon.sprite = evt.icon;

        panel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(HideAfterDelay());
    }

    private void DisplayEventEnd(ArenaEventSO evt)
    {
        eventTitle.text = evt.eventName + " Ended";
        eventDescription.text = "The effect has worn off.";
        panel.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(showDuration);
        panel.SetActive(false);
    }
}
