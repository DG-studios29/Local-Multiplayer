using UnityEngine;
using System.Collections;

public class PlayerCurrency : MonoBehaviour
{
    [SerializeField]private float maxMana;
    [SerializeField]private float currentMana;


    private float playerXP;


    private float manaTimer;
    private float manaGainTime;
    private float manaGainAmount;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentMana = maxMana;

    }

    // Update is called once per frame
    void Update()
    {
        manaTimer += Time.deltaTime;


        ManaRegeneration();
    }

    public void ManaLoss(float amount)
    {
        currentMana -= amount;

        if(currentMana < 0) currentMana = 0;

        //Update any ui
    }

    public void ManaGain(float amount)
    {
        currentMana += amount;

        if (currentMana > maxMana) currentMana = maxMana;

        //Update any ui
    }


    void ManaRegeneration()
    {
        if(manaTimer >= manaGainTime)
        {
            ManaGain(manaGainAmount);
            manaTimer = 0;
        }
    }


}
