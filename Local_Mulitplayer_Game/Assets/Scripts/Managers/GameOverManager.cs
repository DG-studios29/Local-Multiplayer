using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    private UINavigation navigation;
    public GameObject playerWinner1; 
    public GameObject playerWinner2; 
    public GameObject draw; 

    void Start()
    {
        // Try to find the UINavigation component
        navigation = Object.FindFirstObjectByType<UINavigation>();

        if (navigation == null)
        {
            Debug.LogError("UINavigation not found! Make sure it is in the scene.");
            return;
        }
        
        string winnerName = PlayerPrefs.GetString("Winner", "No winner");

        
        if (winnerName == "Player 1 wins!") // Change this condition based on how you set player names
        {
            navigation.ShowPanel1();
            playerWinner1.SetActive(true);
            playerWinner2.SetActive(false); 

        }
        else if (winnerName == "Player 2 wins!")
        {
            navigation.ShowPanel2();
            playerWinner1.SetActive(false); 
            playerWinner2.SetActive(true);
        }
        else if (winnerName == "It's a draw!")
        {
            draw.SetActive(true);
            playerWinner1.SetActive(false);
            playerWinner2.SetActive(false);
        }

        
        PlayerPrefs.DeleteKey("Winner");
    }
}
