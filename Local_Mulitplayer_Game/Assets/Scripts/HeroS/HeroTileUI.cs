using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HeroTileUI : MonoBehaviour
{
    [Header("Hero Info")]
    public string heroName;
    public Sprite heroIcon;

    [Header("UI Elements")]
    public TextMeshProUGUI nameLabel;
    public Image portraitImage;

    private void Start()
    {
        // Set the text and image when the tile initializes
        if (nameLabel != null)
            nameLabel.text = heroName;

        if (portraitImage != null && heroIcon != null)
            portraitImage.sprite = heroIcon;
    }
}
