using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HeroTileUI : MonoBehaviour
{
    public string heroName;
    public Sprite heroIcon;
    public TextMeshProUGUI nameLabel;

    void Start()
    {
        if (nameLabel != null)
            nameLabel.text = heroName;
    }
}
