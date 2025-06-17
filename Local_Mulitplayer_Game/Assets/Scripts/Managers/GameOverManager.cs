using TMPro;
using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance;

    public TMP_Text resultText;
    public GameObject gameOverPanel;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public static void ShowResult(string result)
    {
        // Show result in UI (mock version)
        Debug.Log($"[GAME OVER UI] {result}");

        // Add your UI screen logic here
        if (Instance != null && Instance.resultText != null && Instance.gameOverPanel != null)
        {
            Instance.resultText.text = result;
            Instance.gameOverPanel.SetActive(true);
        }
    }
}
