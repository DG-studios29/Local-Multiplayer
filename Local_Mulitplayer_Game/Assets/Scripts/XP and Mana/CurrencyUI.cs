using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField]private Slider manaFillImage;
    [SerializeField]private TextMeshProUGUI xpText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ManaBarUpdate(1f);
        XpTextUpdate(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ManaBarUpdate(float fillRatio)
    {
        if(!manaFillImage) return;
        manaFillImage.value = fillRatio;
        
        var fillPercentage = fillRatio * 100f;
        fillPercentage = Mathf.RoundToInt(fillPercentage);
        
        //xpText.text = fillPercentage.ToString("F2") + "%";
        xpText.text = fillPercentage.ToString("F2") + "%";
    }

    public void XpTextUpdate(float xp)
    {
        //xpText.text = "XP : " + xp;
    }

   
    
}
