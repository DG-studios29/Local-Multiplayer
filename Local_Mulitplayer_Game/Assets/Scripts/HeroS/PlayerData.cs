using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    public static List<PlayerStats> players = new List<PlayerStats>();

    public static void SavePlayerStats(GameObject player)
    {
        PlayerHealth health = player.GetComponent<PlayerHealth>();
        PlayerStats stats = new PlayerStats
        {
            playerName = player.name,
            health = health.currentHealth,
            //heroType = player.GetComponent<HeroBase>().GetType().Name
        };

        players.Add(stats);
    }

    public static void ClearData()
    {
        players.Clear();
    }
}

public class PlayerStats
{
    public string playerName;
    public int health;
    public string heroType;
}
