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
    public Image activeIcon;
    public GameObject gbActiveIcon;
    public Image fillImage;  
    private Coroutine fillCoroutine;

    public float showDuration = 4f;

    private void OnEnable()
    {
        ArenaEventManager.OnArenaEventStart += DisplayEvent;
        ArenaEventManager.OnArenaEventEnd += DisplayEventEnd;
        ArenaEventManager.OnArenaEventStart += ShowActiveIcon;
        ArenaEventManager.OnArenaEventEnd += HideActiveIcon;
    }

    private void OnDisable()
    {
        ArenaEventManager.OnArenaEventStart -= DisplayEvent;
        ArenaEventManager.OnArenaEventEnd -= DisplayEventEnd;
        ArenaEventManager.OnArenaEventStart -= ShowActiveIcon;
        ArenaEventManager.OnArenaEventEnd -= HideActiveIcon;
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

    private void ShowActiveIcon(ArenaEventSO evt)
    {
        if (activeIcon != null && evt.icon != null)
        {
            activeIcon.sprite = evt.icon;
            activeIcon.fillAmount = 1f;
            activeIcon.enabled = true;

            if (fillCoroutine != null) StopCoroutine(fillCoroutine);
            fillCoroutine = StartCoroutine(FillCountdown(evt.duration));

            gbActiveIcon.SetActive(true); // Enable the icon's GameObject
        }
    }

    private IEnumerator FillCountdown(float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            activeIcon.fillAmount = 1f - (elapsed / duration);
            yield return null;
        }

        activeIcon.fillAmount = 0f;
    }

    private void HideActiveIcon(ArenaEventSO evt)
    {
        if (fillCoroutine != null) StopCoroutine(fillCoroutine);
        activeIcon.fillAmount = 0f;
        activeIcon.enabled = false;
        gbActiveIcon.SetActive(false);
    }

}
