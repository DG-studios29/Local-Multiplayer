using UnityEngine;
using System.Collections;

public class ItemShoot : ItemObject
{


    protected GameObject trailRend;
    protected GameObject projectileObject;
    protected GameObject particleFX;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        Debug.Log("called");

        trailRend = itemData.trailRenderer;
        projectileObject = itemData.projectilePrefab;
        particleFX = itemData.particleFX;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void DoAttack()
    {
        if (s_timerSinceAttack > s_attackRate)
        {
            //if enough time has elapsed
            FindNearest();

            //if the nearest enemy is within range
            if (nearestTarget != null)
            {
                if (Vector3.Distance(gameObject.transform.position, nearestTarget.transform.position) <= radius)
                {
                    // Visually Show something
                    GameObject shotTrail = GameObject.Instantiate(trailRend,this.transform.position,Quaternion.identity);
                    StartCoroutine(DrawTrail(shotTrail,nearestTarget.transform.position));

                    //Take Damage
                    nearestTarget.GetComponent<EnemyAI>().TakeDamage(damage, gameObject);
                    Debug.Log("Dealing damage to: \t" + nearestTarget.name);

                    s_timerSinceAttack = 0; //reset the timer
                    nearestTarget = null; //nulling the nearest target
                }
            }

        }

    }

    private IEnumerator DrawTrail(GameObject trail, Vector3 targetPos)
    {
        float percentageComplete = 0f;
        float time = 0.1f;

        Vector3 trailStartPosition = trail.transform.position;

        while (percentageComplete < 1)
        {
            percentageComplete += Time.deltaTime / time;

            if (percentageComplete > 1) percentageComplete = 1;


            trail.transform.position = Vector3.Lerp(trailStartPosition, targetPos, percentageComplete);

            yield return null;
        }

        //trail.transform.position = hit.transform.position;
        //Destroy(bulletTrail.gameObject, 0.1f);
        Destroy(trail.gameObject, 1f);

    }
}
