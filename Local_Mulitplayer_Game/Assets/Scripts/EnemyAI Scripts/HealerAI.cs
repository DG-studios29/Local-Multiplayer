using UnityEngine;

public class HealerAI : EnemyAI
{
    //private GameObject ;
    private float distanceToHealer;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        navAgent.stoppingDistance = 3f; 
    }

    // Update is called once per frame
    protected override void Update()
    {
        time_sinceAttack += Time.deltaTime; //time since healed
        
        DoTargetChase();

        if (time_sinceAttack > attackCooldown)
        {
            DoHeal();
        }
    }

    protected override void DoAttack()
    {
        //Does not attack
        return;
    }

    protected override void DoTargetChase()
    {
        distanceToHealer = Vector3.Distance(transform.position,enemyParent.transform.position);
        navAgent.SetDestination(enemyParent.transform.position);
    }

    private void DoHeal()
    {
        //if within distance specified as attack range, will start healing
        if (distanceToHealer < attackRange)
        {
            CascadeAnimation();
            
            //Will heal by the damage specified in enemy data
            enemyParent.GetComponent<PlayerHealth>().Heal((int)damage);
            time_sinceAttack = 0;
        }
        
    }
    
}
