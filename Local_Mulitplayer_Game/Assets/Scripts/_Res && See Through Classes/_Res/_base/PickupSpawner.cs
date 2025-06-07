using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This class handles how resources are spawned and managed
/// </summary>
public class PickupSpawner : MonoBehaviour, ISpawnable
{
    #region Custom Variables

    public static PickupSpawner instance;

    [Header("Pickups Properties"), Space(10f)]

    [SerializeField] private GameObject [] pickupPrefabs;
    private int randomPoint, randomPickUp;
    private float randomWaitTime;
    
    public bool isSpawning = false;

    public int currentlySpawnedPickUps = 0;
    private int maxSpawnedPickUp = 10;

    [SerializeField] private LayerMask filter;

    #endregion

    #region Built- In Methods
    void Awake()
    {
        if (instance == null) instance = this;
        StartCoroutine(Force());
    }

    private void OnEnable()
    {
        isSpawning = false;
    }
    private IEnumerator Force()
    {
        isSpawning = false;  
        yield return new WaitForSeconds(10f);
        isSpawning = false ;
    }
    #endregion

    #region Interface Methods
    public void DoSpawn(List<Vector3> positions)
    {
        if(!isSpawning && currentlySpawnedPickUps< maxSpawnedPickUp)
        {
            isSpawning = true;
            StartCoroutine(SpawnIntervals(positions));
        }      
    }

    #endregion

    #region  Custom Methods

    private IEnumerator SpawnIntervals(List<Vector3> positions)
    {
        randomWaitTime = Random.Range(5f, 10);

        yield return new WaitForSeconds(randomWaitTime);

        List<Vector3> validatedPositions = new List<Vector3>();

        for(int i = 0; i< positions.Count; i++)
        {
            if (ValidatePos(positions[i]))
            {
                validatedPositions.Add(positions[i]);
            }
        }

        //here we check if the size of validated positions list == 0
        //if it is, we exit the coroutine early and set the isSpawning bool to false 
        // so no spawning happens
        
        if(validatedPositions.Count == 0)
        {
            isSpawning = false;
            print("bbbbb");
            yield break;    
        }

        randomPickUp = Random.Range(0, pickupPrefabs.Length);
        randomPoint = Random.Range(0, validatedPositions.Count);

        HandlePickUpSpawn(randomPickUp, randomPoint, validatedPositions);
        currentlySpawnedPickUps++;

        isSpawning = false;
        print("bbb");
    }

    private bool ValidatePos(Vector3 pos)
    {
        return !Physics.CheckSphere(pos, 0.5f, filter);
    }

    private void HandlePickUpSpawn(int pickupIndex, int positionIndex, List<Vector3> positions)
    {
        GameObject pickUp = Instantiate(pickupPrefabs[pickupIndex], positions[positionIndex], Quaternion.identity);
        PickUpsBase pickUpClass = pickUp.GetComponent<PickUpsBase>();
        if(pickUpClass != null)
        {
            string name = pickUpClass.GetType().Name;
            pickUp.name = name;
        }
    }

    #endregion
}
