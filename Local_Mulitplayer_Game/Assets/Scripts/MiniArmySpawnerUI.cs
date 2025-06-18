using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MiniArmySpawnerUI : MonoBehaviour
{
    [System.Serializable]
    public class UnitUI
    {
        public Image cooldownFill;
        public TMP_Text cooldownText;
    }

    public UnitUI[] unitUIs = new UnitUI[4]; // 0 = Melee, 1 = Ranged, 2 = Defense

    // Separate cooldowns for each unit type
    public float[] spawnCooldowns = new float[4] { 2f, 3f, 5f, 1f }; // You can tweak these
    private float[] cooldownTimers = new float[4];

    private Color readyColor = Color.white;
    private Color cooldownColor = Color.gray;

    void Update()
    {
        for (int i = 0; i < 4; i++)
        {
            if (cooldownTimers[i] > 0)
            {
                cooldownTimers[i] -= Time.deltaTime;

                float ratio = Mathf.Clamp01(cooldownTimers[i] / spawnCooldowns[i]);
                unitUIs[i].cooldownFill.fillAmount = ratio;
                unitUIs[i].cooldownFill.color = cooldownColor;

                unitUIs[i].cooldownText.gameObject.SetActive(true);
                unitUIs[i].cooldownText.text = Mathf.CeilToInt(cooldownTimers[i]).ToString();
            }
            else
            {
                unitUIs[i].cooldownFill.fillAmount = 0f;
                unitUIs[i].cooldownFill.color = readyColor;
                unitUIs[i].cooldownText.gameObject.SetActive(false);
            }
        }
    }

    public bool CanSpawn(int index)
    {
        return cooldownTimers[index] <= 0f;
    }

    public void OnUnitSpawned(int index)
    {
        cooldownTimers[index] = spawnCooldowns[index];
    }

    public void SetupInitialUI()
    {
        for (int i = 0; i < 4; i++)
        {
            unitUIs[i].cooldownFill.fillAmount = 0f;
            unitUIs[i].cooldownFill.color = readyColor;
            unitUIs[i].cooldownText.gameObject.SetActive(false);
        }
    }
}
