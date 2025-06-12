using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;


public class EnemyAI : MonoBehaviour
{
    public EnemyData enemyData;

    protected NavMeshAgent navAgent;
    public GameObject enemyParent; //a ref of something like this to check if friend or foe, will be set when spawned by the player

    public List<GameObject> targetList;
    public List<GameObject> playerTargetList;

    public static event Action<GameObject> onEnemySpawn; //event to invoke enemy spawn
    public static event Action<GameObject> onEnemyDeath;

    public static event Action<EnemyAI> findEnemyTargets; //will add all existing enemy types

    [SerializeField] protected float health;
    protected float damage;
    protected float speed;
    protected float targetPlayerRange;
    protected float openRange;

    protected float attackRange;
    protected float attackCooldown;
    protected float time_sinceAttack = 0f;

    [SerializeField] protected GameObject nearestTarget;
    protected float nearestDistance;

    [SerializeField] protected GameObject nearestPlayerTarget;
    protected float nearestPlayerDistance;

    private bool isFrozen;
    private float freezeDuration;

    private string playerTag = "Player";

    //Feedback on K.I.D.S
    protected ArmyControls[] armyControls;

    

    private void OnEnable()
    {
        ItemObject.findEnemies += AddToEnemyList;

        EnemyAI.onEnemySpawn += AddToMyTargetList;
        EnemyAI.onEnemyDeath += RemoveFromMyTargetList;

        EnemyAI.findEnemyTargets += AddToMyTargetList;
    }

    private void OnDisable()
    {
        ItemObject.findEnemies -= AddToEnemyList;

        EnemyAI.onEnemySpawn -= AddToMyTargetList;
        EnemyAI.onEnemyDeath -= RemoveFromMyTargetList;

        EnemyAI.findEnemyTargets -= AddToMyTargetList;
    }


    protected virtual void Start()
    {

        navAgent = GetComponent<NavMeshAgent>();
        GetEnemyData();

        navAgent.speed = speed;

        time_sinceAttack = 0f;

        if (enemyParent == null)
        {
            Debug.LogError($"[EnemyAI] {name} is missing an enemyParent! This will cause targeting issues.");
            return;
        }

        FindEnemyPlayers(); //A bit of a waste, randomize parent does the same thing ; fix later

        onEnemySpawn?.Invoke(this.gameObject); // alert all listeners that this enemy is spawned

        findEnemyTargets?.Invoke(this); // find all enemy targets that exist elsewhere

        armyControls = GetComponentsInChildren<ArmyControls>(); // smooth way to control our armies        

    }




    // Update is called once per frame
    protected virtual void Update()
    {
        time_sinceAttack += Time.deltaTime;

        //might keep the tracking functions to a global interval or something like that
        NearestTargetTracking();
        NearestPlayerTracking();

        DoTargetChase();

        if (time_sinceAttack > attackCooldown)
        {
            DoAttack();
        }


    }

    protected virtual void EnemyDestroy()
    {

        Destroy(gameObject, 0.1f);
    }

    public void TakeDamage(float hp)
    {
        health -= hp;
        Debug.Log($"Enemy {gameObject.name} took {hp} damage! Current HP: {health}");

        foreach(ArmyControls party in armyControls)
        {
            party.ShowDamage();
        }

        if(health <= 0)
        {
            onEnemyDeath?.Invoke(this.gameObject);
            EnemyDestroy();
        }
    }


    void GetEnemyData()
    {
        health = enemyData.MaxHealth;
        damage = enemyData.Damage;
        speed = enemyData.MoveSpeed;
        targetPlayerRange = enemyData.TargetPlayerRange;
        openRange = enemyData.OpenRange;
        attackRange = enemyData.AttackRange;

        attackCooldown = enemyData.AttackRate;
    }


    void FindEnemyPlayers()
    {
        GameObject[] gameObjects = GameObject.FindGameObjectsWithTag(playerTag);

        foreach (GameObject parent in gameObjects)
        {
            if (parent != enemyParent)  //dont add your own parent
                playerTargetList.Add(parent);
        }
    }



    //finds all existing enemies, and stores them in the item object's list on its awake
    public void AddToEnemyList(ItemObject itemObject)
    {
        itemObject.AddEnemyToTarget(this.gameObject);
    }



    //Enemies can add other enemies to their list of targets
    public void AddToMyTargetList(GameObject enemy)
    {
        //needs to check if its already in the list, if they are not of the same parent, and if it is not itself


        //make sure a parent exists // wont be a problem when linked to spawning
        if (enemyParent == null)
        {
            Debug.LogWarning($"[EnemyAI] No enemyParent assigned to {name}. Skipping playerTarget setup.");
            return;
        }


        if (this.enemyParent != enemy.GetComponent<EnemyAI>().enemyParent)  // if not siblings, add to enemy and target list
        {
            if (targetList.Count > 0)
            {
                bool alreadyTargeting = false;

                foreach (GameObject target in targetList)
                {
                    if (target == enemy)
                    {
                        alreadyTargeting = true; //no duplicates
                    }

                }

                if (!alreadyTargeting) targetList.Add(enemy);  // if we dont find it in out list, we are not already targetting. hence why we add
            }
            else
            {
                targetList.Add(enemy);
                //initialize target tracking
                nearestDistance = Vector3.Distance(transform.position, targetList[0].transform.position); //first element will be used as nearest
                nearestTarget = targetList[0];  // to avoid a case where nearest target does not end up getting set and thus remains null
            }

        }
        else
        {
            //Debug.Log("Object parent :  " + parentPlayer + "Enemy :  " + enemy.GetComponent<EnemyAI>().enemyParent);
        }
    }

    public void AddToMyTargetList(EnemyAI enemyAI)
    {
        enemyAI.AddToMyTargetList(this.gameObject);
    }


    //Will remove destoryed enemies from list of targets
    public void RemoveFromMyTargetList(GameObject enemy)
    {
        bool removable = false;
        foreach (GameObject target in targetList)
        {
            if (target == enemy)
            {
                //enemyTargets.Remove(target);  //remove an enemy from the list when its destroyed
                removable = true;
            }
        }

        if (removable) targetList.Remove(enemy);
    }


    void NearestTargetTracking()
    {

        if (targetList.Count == 0)
        {
            nearestTarget = null;
            return;
        }
        else if (targetList.Count == 1)
        {
            if (targetList[0] != null)
            {
                nearestDistance = Vector3.Distance(transform.position, targetList[0].transform.position); //first element will be used as nearest
            }
            nearestTarget = targetList[0];  // to avoid a case where nearest target does not end up getting set and thus remains null


        }
        else
        {
            foreach (GameObject target in targetList)
            {
                //Missisng reference error
                //maybe remove from the list then destroy
                if (target != null)
                {
                    var distance = Vector3.Distance(transform.position, target.transform.position);
                    if (distance < nearestDistance)
                    {
                        nearestDistance = distance;
                        nearestTarget = target;
                    }
                }

            }
        }


    }

    void NearestPlayerTracking()
    {
        if (playerTargetList.Count == 0)
        {
            nearestPlayerTarget = null;
            return;
        }
        else if (playerTargetList.Count == 1)
        {
            nearestPlayerDistance = Vector3.Distance(transform.position, playerTargetList[0].transform.position); //first element will be used as nearest
            nearestPlayerTarget = playerTargetList[0];  // to avoid a case where nearest target does not end up getting set and thus remains null

        }
        else
        {
            foreach (GameObject target in playerTargetList)
            {
                var distance = Vector3.Distance(transform.position, target.transform.position);
                if (distance < nearestPlayerDistance)
                {
                    nearestPlayerDistance = distance;
                    nearestPlayerTarget = target;
                }
            }
        }
    }

    //needs to make logical decisions in all posible cases of what to target
    protected virtual void DoTargetChase()
    {
        if (nearestTarget == null && nearestPlayerTarget != null)
        {
            //We have no other enemies to worry about and will just target the player 
            navAgent.SetDestination(nearestPlayerTarget.transform.position);

            /* if (nearestPlayerDistance < targetPlayerRange)
             {
                 navAgent.SetDestination(nearestPlayerTarget.transform.position);
             }*/
        }

        if (nearestTarget != null && nearestPlayerTarget != null)
        {
            //chase the player if its within a certain range no matter what
            if (nearestPlayerDistance < targetPlayerRange)
            {
                navAgent.SetDestination(nearestPlayerTarget.transform.position);
            }

            //else, target the nearest enemy mob
            else if (nearestDistance < nearestPlayerDistance)
            {
                navAgent.SetDestination(nearestTarget.transform.position);
            }
            //or player
            else
            {
                navAgent.SetDestination(nearestPlayerTarget.transform.position);
            }
        }

        //outlier cases
        if (nearestTarget != null && nearestPlayerTarget == null)
        {
            navAgent.SetDestination(nearestTarget.transform.position);
        }

        if (nearestTarget == null && nearestPlayerTarget == null)
        {
            navAgent.SetDestination(this.transform.position);
        }


    }

    //maybe edit distance, target convention here and use etc (distance of nearest target)? nahh
    protected virtual void DoAttack()
    {

        if (nearestTarget != null && nearestPlayerTarget != null)
        {
            if (nearestDistance < nearestPlayerDistance && nearestDistance < attackRange)
            {

                CascadeAnimation();

                nearestTarget.GetComponent<EnemyAI>().TakeDamage(damage); //deal damage when in attack range
                time_sinceAttack = 0; //sets the cooldown for this function's condition
            }
            else if (nearestPlayerDistance < nearestDistance && nearestPlayerDistance < attackRange)
            {
                CascadeAnimation();

                //player takes damage
                nearestPlayerTarget.GetComponent<PlayerHealth>().TakeDamage((int)damage);
                time_sinceAttack = 0;
            }
        }

        else if (nearestTarget == null && nearestPlayerTarget != null)
        {
            if (nearestPlayerDistance < attackRange)
            {
                CascadeAnimation();

                //player to take damage
                nearestPlayerTarget.GetComponent<PlayerHealth>().TakeDamage((int)damage);
                time_sinceAttack = 0;
            }
        }
        else if (nearestTarget != null && nearestPlayerTarget == null)
        {
            if (nearestDistance < attackRange)
            {
                CascadeAnimation();

                nearestTarget.GetComponent<EnemyAI>().TakeDamage(damage);
                time_sinceAttack = 0;
            }
        }

    }

    protected void CascadeAnimation()
    {
        foreach (ArmyControls party in armyControls)
        {
            party.AnimateAttack();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, targetPlayerRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, openRange);


    }

    public void Freeze(float duration)
    {
        if (isFrozen) return; 
        isFrozen = true;
        freezeDuration = duration;
        // Apply freezing logic, e.g., stop movement 
        GetComponent<NavMeshAgent>().isStopped = true;

        StartCoroutine(FreezeDuration());
    }

    private IEnumerator FreezeDuration()
    {
        yield return new WaitForSeconds(freezeDuration);
        // After the freeze duration ends, re-enable movement
        GetComponent<NavMeshAgent>().isStopped = false;
        isFrozen = false;
    }
}
