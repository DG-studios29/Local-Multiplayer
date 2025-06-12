using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField]private Image manaFillImage;
    [SerializeField]private TextMeshProUGUI xpText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ManaBarUpdate(1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ManaBarUpdate(float fillRatio)
    {
        if(!manaFillImage) return;
        manaFillImage.fillAmount = fillRatio;
    }

    public void UpdateXpText(float xp)
    {
        xpText.text = "XP: " + xp;
    }
    
}
