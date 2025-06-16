using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ArenaEventManager : MonoBehaviour
{
    public static ArenaEventManager Instance;

    public List<ArenaEventSO> availableEvents;
    public AnimationCurve pacingCurve; 

    private ArenaEventSO currentEvent;

    private int maxEvents = 4;
    private float minDelay = 30f;
    private float maxDelay = 40f;

    private float matchTimer = 0f;
    private float matchDuration = 300f;

    public ArenaEventSO CurrentEvent => currentEvent;

    public delegate void EventTrigger(ArenaEventSO triggeredEvent);
    public static event EventTrigger OnArenaEventStart;
    public static event Action<ArenaEventSO> OnArenaEventEnd;

    private void Awake() => Instance = this;

    private void Start()
    {
        SetupMapPacing();
        StartCoroutine(EventRoutine());
    }

    private void Update()
    {
        matchTimer += Time.deltaTime;
    }

    IEnumerator EventRoutine()
    {
        yield return new WaitForSeconds(10f); // Initial delay

        int triggered = 0;

        while (triggered < maxEvents)
        {
            if (currentEvent == null)
            {
                TriggerRandomEvent();
                triggered++;

                //pacing curve to scale delay
                float progress = Mathf.Clamp01(matchTimer / matchDuration);
                float curveMultiplier = pacingCurve != null ? pacingCurve.Evaluate(progress) : 1f;

                float scaledMin = minDelay * curveMultiplier;
                float scaledMax = maxDelay * curveMultiplier;

                float delay = UnityEngine.Random.Range(scaledMin, scaledMax);
                yield return new WaitForSeconds(delay);
            }

            yield return null;
        }
    }

    void TriggerRandomEvent()
    {
        currentEvent = availableEvents[UnityEngine.Random.Range(0, availableEvents.Count)];
        Debug.Log($"Arena Event Triggered: {currentEvent.eventName}");

        OnArenaEventStart?.Invoke(currentEvent);
        StartCoroutine(EndEventAfterDuration(currentEvent));
    }

    IEnumerator EndEventAfterDuration(ArenaEventSO evt)
    {
        yield return new WaitForSeconds(evt.duration);
        Debug.Log($"Arena Event Ended: {evt.eventName}");

        currentEvent = null;
        OnArenaEventEnd?.Invoke(evt);
    }

    public bool IsActive(string flagName)
    {
        if (currentEvent == null) return false;
        var field = typeof(ArenaEventSO).GetField(flagName);
        return field != null && (bool)field.GetValue(currentEvent);
    }

    void SetupMapPacing()
    {
        string currentMap = GameManager.Instance.CurrentMapName;

        switch (currentMap)
        {
            case "Forest":
                maxEvents = 4;
                minDelay = 30f;
                maxDelay = 40f;
                break;
            case "Cemetery":
                maxEvents = 6;
                minDelay = 25f;
                maxDelay = 35f;
                break;
            case "Winter":
                maxEvents = 3;
                minDelay = 45f;
                maxDelay = 55f;
                break;
            default:
                maxEvents = 4;
                minDelay = 30f;
                maxDelay = 40f;
                break;
        }
    }
}



//#if UNITY_EDITOR
//    [ContextMenu("Trigger Selected Test Event")]
//    public void TriggerTestEvent()
//    {
//        if (testEvent == null)
//        {
//            Debug.LogWarning("ArenaEventManager: No testEvent assigned!");
//            return;
//        }

//        StopAllCoroutines(); // Stop main event routine
//        currentEvent = testEvent;

//        Debug.Log("Manually triggered test event: " + testEvent.eventName);
//        OnArenaEventStart?.Invoke(testEvent);
//        StartCoroutine(EndEventAfterDuration(testEvent));
//    }

//    // Add this field to select a test event from the list
//    public ArenaEventSO testEvent;
//#endif


