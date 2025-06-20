﻿using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;

public class MiniArmySpawner : MonoBehaviour
{
    public GameObject[] armyTypes; // 0 = Melee, 1 = Ranged, 2 = Defense
    public Transform spawnPoint;

    [Header("Spam Spawn Settings")]
    public float spamSpawnInterval = 3f;

    private MiniArmySpawnerUI ui;
    private Coroutine spamRoutine;
    
    private PlayerCurrency pCurrency;

    void Start()
    {
        pCurrency = GetComponent<PlayerCurrency>();
    }
    
    private void OnEnable()
    {
        ArenaEventManager.OnArenaEventStart += OnEventStart;
        ArenaEventManager.OnArenaEventEnd += OnEventEnd;
    }

    private void OnDisable()
    {
        ArenaEventManager.OnArenaEventStart -= OnEventStart;
        ArenaEventManager.OnArenaEventEnd -= OnEventEnd;
    }

    private void OnEventStart(ArenaEventSO evt)
    {
        if (evt.triggerSpamSpawner && spamRoutine == null)
        {
            spamRoutine = StartCoroutine(SpamSpawnRoutine());
        }
    }

    private void OnEventEnd(ArenaEventSO evt)
    {
        if (evt.triggerSpamSpawner && spamRoutine != null)
        {
            StopCoroutine(spamRoutine);
            spamRoutine = null;
        }
    }

    private IEnumerator SpamSpawnRoutine()
    {
        while (true)
        {
            int index = Random.Range(0, armyTypes.Length);
            SpawnMiniUnit(index);
            yield return new WaitForSeconds(spamSpawnInterval);
        }
    }

    public void InitializeUI(MiniArmySpawnerUI assignedUI)
    {
        ui = assignedUI;
        ui.SetupInitialUI();
        
        
    }

    public void SpawnArmyByKey1(InputAction.CallbackContext context)
    {
        if (context.performed) TrySpawn(0);
        TutorialActionLinq("SpawnBase");
    }

    public void SpawnArmyByKey2(InputAction.CallbackContext context)
    {
        if (context.performed) TrySpawn(1);
        TutorialActionLinq("SpawnDemon");
    }

    public void SpawnArmyByKey3(InputAction.CallbackContext context)
    {
        if (context.performed) TrySpawn(2);
        TutorialActionLinq("SpawnTank");
    }

    public void SpawnArmyByKey4(InputAction.CallbackContext context)
    {
        if (context.performed) TrySpawn(3);
        TutorialActionLinq("SpawnHealer");
    }

    private void TrySpawn(int index)
    {
        if (ui != null && ui.CanSpawn(index))
        {

            var typeToSpawn = armyTypes[index].GetComponent<EnemyAI>().enemyData;
            
            if (pCurrency.CheckManaCost(typeToSpawn.ManaCost) == false)
            {
                Debug.Log($"Can't spawn {typeToSpawn.ManaCost}");
                //We cannot pay the cost and must therefore exit
                return;
            }

            pCurrency.ManaLoss(typeToSpawn.ManaCost);


            int spawnCount = 1;
            if (ArenaEventManager.Instance != null && ArenaEventManager.Instance.IsActive("triggerDoubleArmy"))
            {
                spawnCount = 2;
            }

            for (int i = 0; i < spawnCount; i++)
            {
                SpawnMiniUnit(index);
            }

            ui.OnUnitSpawned(index);
        }
    }

    private void SpawnMiniUnit(int index)
    {
        GameObject unit = Instantiate(armyTypes[index], spawnPoint.position, spawnPoint.rotation);

        var ai = unit.GetComponent<EnemyAI>();
        if (ai != null)
        {
            ai.enemyParent = gameObject;
        }

        // Assign material from player to spawned unit
        Renderer[] playerRenderers = GetComponentsInChildren<Renderer>();
        Renderer[] unitRenderers = unit.GetComponentsInChildren<Renderer>();

        if (playerRenderers.Length > 0 && unitRenderers.Length > 0)
        {
            Material playerMat = playerRenderers[0].sharedMaterial;

            foreach (Renderer rend in unitRenderers)
            {
                rend.sharedMaterial = playerMat;
            }
        }
    }
    
    private void TutorialActionLinq(string context)
    {
        //Checking if its null
        if (!TutorialManager.instance) return;
        if (TutorialManager.instance.isTutorialActive)
        {
            TutorialManager.instance.CheckTutorialPerform(context);
        }
    }
    
    
}
