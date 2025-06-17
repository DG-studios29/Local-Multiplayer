using UnityEngine;
using System.Collections;

public class HealerAI : EnemyAI
{
    //private GameObject ;
    private float distanceToHealer;
    private bool healing = false;

    [SerializeField]private GameObject healingTrail;
    [SerializeField] private float trailTime = 0.1f;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        //navAgent.stoppingDistance = 11f; 
        transform.LookAt(enemyParent.transform.position);
    }

    // Update is called once per frame
    protected override void Update()
    {
        time_sinceAttack += Time.deltaTime; //time since healed
        
        //DoTargetChase();

        DoLookAt();
    }

    private void DoLookAt()
    {
        transform.LookAt(enemyParent.transform.position);
    }

    protected override void DoAttack()
    {
        //Does not attack
    }

    protected override void DoTargetChase()
    {
        distanceToHealer = Vector3.Distance(transform.position,enemyParent.transform.position);
        
        //if within distance specified as attack range, will start healing and stop running
        if (distanceToHealer < attackRange)
        {
            
            navAgent.SetDestination(transform.position);
            
            //Do Idle-Healing animation
            CascadeHealing();
            
            if (time_sinceAttack > attackCooldown)
            {
                 DoHeal();
            }
        }
        else
        {
            //Stop Healing Animation
            CascadeStopHealing();
            
            navAgent.SetDestination(enemyParent.transform.position);
        }
        
    }

    private void DoHeal()
    {
        
            if (!healing)
            {
                //Attack animation 
                //CascadeHealing();
            }

            //Trail moving towards parent to indicate healing
            // Visually Show something
            GameObject trailObj = GameObject.Instantiate(healingTrail,this.transform.position + new Vector3(0,1,0),Quaternion.identity);
            StartCoroutine(DrawTrail(trailObj,enemyParent.transform.position,trailTime));
            
            //prevent multiple calls while coroutine runs
            time_sinceAttack = 0;
            
            /*//Will heal by the damage specified in enemy data
            enemyParent.GetComponent<PlayerHealth>().Heal((int)damage);
            time_sinceAttack = 0;*/
        
    }
    
    
    private IEnumerator DrawTrail(GameObject trail, Vector3 targetPos, float duration)
    {
        healing = true;
        float percentageComplete = 0f;
        float time = duration;

        Vector3 trailStartPosition = trail.transform.position;

        while (percentageComplete < 1)
        {
            percentageComplete += Time.deltaTime / time;

            if (percentageComplete > 1) percentageComplete = 1;


            trail.transform.position = Vector3.Lerp(trailStartPosition, targetPos, percentageComplete);

            yield return null;
        }
        
        //Will heal by the damage specified in enemy data
        enemyParent.GetComponent<PlayerHealth>().Heal((int)damage);
        

        //trail.transform.position = hit.transform.position;
        //Destroy(bulletTrail.gameObject, 0.1f);
        Destroy(trail.gameObject, 1f);
        healing = false;
        
       

    }
    
    protected void CascadeHealing()
    {
        foreach (ArmyControls party in armyControls)
        {
            party.AnimateHealing();
        }
    }

    protected void CascadeStopHealing()
    {
        foreach (ArmyControls party in armyControls)
        {
            party.AnimateStopHealing();
        }
    }
    
}
