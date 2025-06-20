﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AutoAttack : MonoBehaviour, IPlayerEffect
{
    public string playerTag = "Player";  //will latch onto this object's transform

    //public GameObject playerTestObj;
    //public Transform playerObjOrigin;

    public List<ItemData> itemHolder;
    public int itemSize;

    public GameObject autoShootObject;
    public GameObject revolverObject;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPunches.OnlyPunchesActive)
        {
            Debug.Log("AutoAttack disabled due to Only Punches event.");
            return; // Don't spawn auto-attacks
        }

        ItemHolder(); 
        foreach (ItemData item in itemHolder)
        {
            Instantiate(item.objectInstance, this.transform.position, Quaternion.identity, this.transform);
        }

        /*playerTestObj = GameObject.FindGameObjectWithTag(playerTag);

        if (playerTestObj != null)
        {
            playerObjOrigin = playerTestObj.transform;
            transform.SetParent(playerObjOrigin);


            this.transform.position = playerObjOrigin.position;
        }
*/
        //ItemHolder();  // initialising the item slots

        // will fix this to make it cleaner, and place it in a function for it to be dynamic
        //itemHolder.Add(basicAutoBB.GetComponent<ItemObject>());
        //Instantiate(itemHolder[0].itemData.objectInstance, this.transform.position, Quaternion.identity, this.transform);

        //itemHolder.Add(revolverObject.GetComponent<ItemObject>());

        /*   foreach(ItemObject item in itemHolder)
           {
               //Instantiate(itemHolder[0].itemData.objectInstance, this.transform.position, Quaternion.identity, this.transform);
               Instantiate(item.itemData.objectInstance, this.transform.position, Quaternion.identity, this.transform);

           }*/


    }


    public void InstantiateAutoShooter(ItemData shootingData)
    {
        //Debug.Log("Auto Shooter Instantiated on creation");
        itemHolder.Add(shootingData);
        Instantiate(shootingData.objectInstance,this.transform.position,Quaternion.identity,this.transform);
    }

    public void InstantiateRevolver(ItemData revolvingData)
    {
        //Debug.Log("Called on creation");
        itemHolder.Add(revolvingData);
        Instantiate(revolvingData.objectInstance, this.transform.position, Quaternion.identity, this.transform);
    }

    public void ItemHolder()
    {
        itemHolder = new List<ItemData>(itemSize);

      /*  for(int i = 0; i < itemSize; i++)
        {
            itemHolder.Add(new ItemObject());
        }*/

    }




    public void TestClear()
    {
        ItemRotate rotateObject = GetComponentInChildren<ItemRotate>();
        rotateObject.RebuildRotor();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = playerObjOrigin.position;
    }

    #region Interface

    public void ActivateSpeedBoost(float duration, float speedMultiplier, GameObject trailEffect)
    {

    }

    public void ActivateShield(float duration, GameObject shield)
    {

    }

    public void GiveHealth(float health)
    {

    }

    public void RestoreOrbs()
    {
        TestClear();
        StartCoroutine(OrbTime());
    }

    public void ResetAbilityCooldownTimer(int cooldown)
    {
        //
    }

    #endregion

    #region Wandile's Methods

    IEnumerator OrbTime()
    {
        switch (transform.root.name)
        {
            case "Player 1":
                GameManager.Instance.playerOnePowerUps[3].alpha = 1;
                break;

            case "Player 2":
                GameManager.Instance.playerTwoPowerUps[3].alpha = 1;
                break;
        }

        yield return new WaitForSeconds(1f);
        switch (transform.root.name)
        {
            case "Player 1":
                GameManager.Instance.playerOnePowerUps[3].alpha = 0.1f;
                break;

            case "Player 2":
                GameManager.Instance.playerTwoPowerUps[3].alpha = 0.1f;
                break;
        }
    }

    #endregion
}
