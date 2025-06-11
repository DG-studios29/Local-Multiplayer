using UnityEngine.InputSystem;
using UnityEngine;

public class MiniArmySpawner : MonoBehaviour
{
    public GameObject[] armyTypes; // 0 = Melee, 1 = Ranged, 2 = Defense
    public Transform spawnPoint;

    private MiniArmySpawnerUI ui;

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
        //will need to check Mana amount, then use up Mana to spawn
        
        
        
        if (ui != null && ui.CanSpawn(index))
        {
            SpawnMiniUnit(index);
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
