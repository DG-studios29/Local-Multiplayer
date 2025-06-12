using UnityEngine.InputSystem;
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

    private PlayerCurrency currency;

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
    

    private void Start()
    {
        currency = GetComponent<PlayerCurrency>();
    }

    public void InitializeUI(MiniArmySpawnerUI assignedUI)
    {
        ui = assignedUI;
        ui.SetupInitialUI();
    }

    public void SpawnArmyByKey1(InputAction.CallbackContext context)
    {
        if (context.performed) TrySpawn(0);
    }

    public void SpawnArmyByKey2(InputAction.CallbackContext context)
    {
        if (context.performed) TrySpawn(1);
    }

    public void SpawnArmyByKey3(InputAction.CallbackContext context)
    {
        if (context.performed) TrySpawn(2);
    }

    private void TrySpawn(int index)
    {
        if (ui != null && ui.CanSpawn(index))
        {
             //will need to check Mana amount required, then use up Mana to spawn
            var spawnInfo = armyTypes[index].GetComponent<EnemyAI>().enemyData;
           
            if (currency.CheckManaCost(spawnInfo.ManaCost))
            {
                currency.ManaLoss(spawnInfo.ManaCost);
            }
            else
            {
                Debug.Log("Not enough mana to spend");
                return;
            }
            

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
    }
}
