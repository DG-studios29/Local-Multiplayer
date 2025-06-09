using UnityEngine;

[CreateAssetMenu(fileName = "NewArenaEvent", menuName = "Arena Events/Arena Event")]
public class ArenaEventSO : ScriptableObject
{
    public string eventName;
    [TextArea(2, 5)] public string description;
    public float duration = 10f;
    public Sprite icon;

    // Hook into gameplay logic
    public bool triggerDoubleArmy;
    public bool triggerOnlyPunches;
    public bool triggerNoHealing;
    public bool triggerReverseControls;
    public bool triggerPowerSurge;
    public bool triggerPortalSpawn;
    public bool triggerSuddenDeath;
    public bool triggerChainReaction;

    // Add more toggles here as needed
}
