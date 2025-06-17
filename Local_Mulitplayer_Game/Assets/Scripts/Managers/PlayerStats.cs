using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int armyKills = 0;


    public void AddArmyKill()
    {
        armyKills++;
        GameManager.Instance.UpdateKillUI();
        
    }

    public int GetArmyKills()
    {
        return armyKills;
    }
}
