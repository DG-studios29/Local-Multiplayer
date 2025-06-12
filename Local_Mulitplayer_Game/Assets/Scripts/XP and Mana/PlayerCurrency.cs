using UnityEngine;
using System.Collections;


public class PlayerCurrency : MonoBehaviour
{
    [SerializeField]private float maxMana;
    [SerializeField]private float currentMana;
    
    private float playerXp;
    
    private float manaTimer = 0f;
    [SerializeField]private float manaGainTime = 0.25f;
    [SerializeField]private float manaGainAmount;
    
    private float xpTimer = 0f;
    private float xpGainTime = 1f;
    private float xpGainAmount = 1;
    private float xpGainMultiplier = Mathf.Exp(1);

    private CurrencyUI currencyUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        currentMana = maxMana;
        playerXp = 0f;

    }

    // Update is called once per frame
    private void Update()
    {
        manaTimer += Time.deltaTime;
        xpTimer += Time.deltaTime;


        ManaRegeneration();
        XpSteadyGain();
    }

    public void SetupManaUI(CurrencyUI playerCurrencyUI)
    {
        currencyUI = playerCurrencyUI;
    }

    public bool CheckManaCost(float amount)
    {
        //true means we can pay the cost
        return currentMana - amount >= 0;
    }
    
    public void ManaLoss(float amount)
    {
        currentMana -= amount;

        if(currentMana < 0) currentMana = 0;

        //Update any ui
        currencyUI.ManaBarUpdate(currentMana/maxMana);
        
    }

    public void ManaGain(float amount)
    {
        currentMana += amount;

        if (currentMana > maxMana) currentMana = maxMana;

        //Update any ui
        currencyUI.ManaBarUpdate(currentMana/maxMana);
    }


    private void ManaRegeneration()
    {
        if (!(manaTimer >= manaGainTime)) return;
        ManaGain(manaGainAmount);
        manaTimer = 0;
    }

    public void XpGain(float amount)
    {
        playerXp += Mathf.Round(amount);
        
        //UI update
        currencyUI.XpTextUpdate(playerXp);
    }
    
    private void XpSteadyGain()
    {
        if(!(xpTimer >= xpGainTime)) return;

        xpGainMultiplier = xpGainMultiplier * Mathf.Exp(1);
        
        xpGainAmount = xpGainAmount * xpGainMultiplier;
        XpGain(xpGainAmount);
    }
    
    

}
