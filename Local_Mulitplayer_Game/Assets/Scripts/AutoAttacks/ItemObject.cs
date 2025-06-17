using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemObject : MonoBehaviour
{
    [SerializeField]public ItemData itemData; //item data to be used

    protected float damage;
    protected float radius;
    protected float s_timerSinceAttack = 0f;
    protected float s_attackRate;

    public float Damage => damage;

    private Transform playerOrigin; //starting postion
    public List<GameObject> enemyTargets;


    [SerializeField]protected GameObject nearestTarget;
    protected float nearestDistance;

    public static event Action<ItemObject> findEnemies;

    public GameObject parentPlayer;

  
    private void OnEnable()
    {
        EnemyAI.onEnemySpawn += AddEnemyToTarget;
        EnemyAI.onEnemyDeath += RemoveEnemyFromTargets;
    }

    private void OnDisable()
    {
        EnemyAI.onEnemySpawn -= AddEnemyToTarget;
        EnemyAI.onEnemyDeath -= RemoveEnemyFromTargets;
    }



    private void Awake()
    {
       
    }


    protected virtual void Start()
    {
        
        s_timerSinceAttack = 0f;

        InitializeObject();

        parentPlayer = GetComponentInParent<PlayerController>().gameObject;

        findEnemies?.Invoke(this);  //find all enemies that currently exist in the scene
        Debug.Log("Called from Item Object");

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        s_timerSinceAttack += Time.deltaTime;

        DoAttack();
    }

    public void InitializeObject()
    {
        //kinda our constructor

        damage = itemData.atkDamage;
        radius = itemData.atkRadius;
        s_attackRate = itemData.atkRate;

        enemyTargets = new List<GameObject>();
    }

    public void AddEnemyToTarget(GameObject enemy)
    {
        
            if (parentPlayer != enemy.GetComponent<EnemyAI>().enemyParent)  // if not siblings, add to enemy and target list
            {
                enemyTargets.Add(enemy);
            }
            else
            {
                //Debug.Log("Object parent :  " + parentPlayer + "Enemy :  " + enemy.GetComponent<EnemyAI>().enemyParent);
            }
        
    }

    public void RemoveEnemyFromTargets(GameObject enemy)
    {
        bool removable = false;
        foreach(GameObject target in enemyTargets)
        {
            if(target == enemy)
            {
                //enemyTargets.Remove(target);  //remove an enemy from the list when its destroyed
                removable = true;
            }
        }

        if (removable) enemyTargets.Remove(enemy);
    }


    protected virtual void DoAttack()
    {

        if (s_timerSinceAttack > s_attackRate)
        {
            //if enough time has elapsed
            FindNearest();

            //if the nearest enemy is within range
            if(nearestTarget != null)
            {
                if(Vector3.Distance(gameObject.transform.position, nearestTarget.transform.position) <= radius)
                {
                    // Visually Show something

                    //Take Damage
                    nearestTarget.GetComponent<EnemyAI>().TakeDamage(damage, gameObject);
                    Debug.Log("Dealing damage to: \t" + nearestTarget.name);

                    s_timerSinceAttack = 0; //reset the timer
                    nearestTarget = null; //nulling the nearest target
                }
            }

        }



    }

    protected void FindNearest()
    {
        if(enemyTargets.Count == 0)
        {
            nearestTarget = null;
            return;
        }
        else
        {
            nearestDistance = Vector3.Distance(transform.position, enemyTargets[0].transform.position); //first element will be used as nearest
            nearestTarget = enemyTargets[0];  // to avoid a case where nearest target does not end up getting set and thus remains null

        }

        foreach(GameObject target in enemyTargets)
        {
            var distance = Vector3.Distance(transform.position,target.transform.position);
            if(distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestTarget = target;
            }
        }
    }
   
   
}
