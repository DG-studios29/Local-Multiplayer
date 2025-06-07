using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SuddenDeathManager : MonoBehaviour
{
    public List<Transform> suddenDeathSpawnPoints;
    public GameObject playerPrefab;

    public float countdownTime = 3f;
    public TextMeshProUGUI countdownText;

    private void Start()
    {
        StartCoroutine(StartSuddenDeath());
    }

    IEnumerator StartSuddenDeath()
    {
        float timer = countdownTime;
        while (timer > 0)
        {
            countdownText.text = "Sudden Death begins in: " + Mathf.Ceil(timer);
            yield return new WaitForSeconds(1f);
            timer--;
        }

        countdownText.gameObject.SetActive(false);
        SpawnPlayers();
    }

    void SpawnPlayers()
    {
        for (int i = 0; i < PlayerData.players.Count; i++)
        {
            if (i < suddenDeathSpawnPoints.Count)
            {
                GameObject player = Instantiate(playerPrefab, suddenDeathSpawnPoints[i].position, Quaternion.identity);
                player.name = PlayerData.players[i].playerName;

                // Restore player health and hero type
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                playerHealth.currentHealth = PlayerData.players[i].health;
                playerHealth.UpdateHealthUI();

                AssignHeroScript(player, PlayerData.players[i].heroType);
            }
        }
    }

    void AssignHeroScript(GameObject player, string heroType)
    {
        System.Type heroClass = System.Type.GetType(heroType);
        if (heroClass != null)
        {
            player.AddComponent(heroClass);
        }
    }
}
