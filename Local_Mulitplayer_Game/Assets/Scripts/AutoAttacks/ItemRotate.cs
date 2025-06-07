using UnityEngine;
using System.Collections;

public class ItemRotate : ItemObject
{
    public GameObject rotationPivot;
    public GameObject rotateObject;
    //public Transform rotationPivot;

    public GameObject pivotInstance;

    private float innerRadius;

    private float buildTime = 0.28f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
     protected override void Start()
    {

        base.Start();
        innerRadius = radius * 0; //within spheres, take damage

        //rotationPivot = new GameObject("RotationPivot");
        pivotInstance = Instantiate(rotationPivot, new Vector3(transform.position.x,transform.position.y +1,transform.position.z), Quaternion.identity,transform); // or just create a transform that sits where we want on the player
        
      
        rotateObject = itemData.projectilePrefab;

        StartCoroutine(SetUpRotor());

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if(pivotInstance != null)
        pivotInstance.transform.Rotate(0, 270 * Time.deltaTime, 0);
    }



    protected override void DoAttack()
    {
        //base.DoAttack();
        //will deal area damage within that radius by attack rate


        //sort of works but will do damage on projectile collisions instead
 /*       if (s_timerSinceAttack > s_attackRate)
        {

            foreach (GameObject target in enemyTargets)
            {
                if (target != null)
                {
                    var tempDistance = Vector3.Distance(gameObject.transform.position, target.transform.position);


                    if (tempDistance <= radius && tempDistance >= innerRadius)
                    {
                        target.GetComponent<EnemyAI>().TakeDamage(damage);
                    }

                }
            }

        }*/


    }


    private IEnumerator SetUpRotor()
    {
       

        GameObject pivotInstanceN = Instantiate(rotationPivot, pivotInstance.transform.position, Quaternion.identity, pivotInstance.transform); // or just create a transform that sits where we want on the player
        GameObject nR = Instantiate(rotateObject, new Vector3(pivotInstanceN.transform.position.x + radius,
                                              pivotInstanceN.transform.position.y,
                                              pivotInstanceN.transform.position.z) ,
            Quaternion.identity,
            pivotInstanceN.transform);

        pivotInstanceN.transform.rotation = Quaternion.Euler(0f,0f,0f);
        //pivotInstanceN.transform.rotation = pivotInstance.
        pivotInstanceN.SetActive(false);



        GameObject pivotInstanceS = Instantiate(rotationPivot, pivotInstance.transform.position, Quaternion.identity, pivotInstance.transform);
        GameObject sR = Instantiate(rotateObject, new Vector3(pivotInstanceS.transform.position.x + radius,
                                              pivotInstanceS.transform.position.y,
                                              pivotInstanceS.transform.position.z) ,
            Quaternion.identity,
            pivotInstanceS.transform);

        pivotInstanceS.transform.rotation = Quaternion.Euler(0f,180f,0f);

        pivotInstanceS.SetActive(false);


        GameObject pivotInstanceW = Instantiate(rotationPivot, pivotInstance.transform.position, Quaternion.identity, pivotInstance.transform);
        GameObject wR = Instantiate(rotateObject, new Vector3(pivotInstanceW.transform.position.x + radius,
                                              pivotInstanceW.transform.position.y,
                                              pivotInstanceW.transform.position.z) ,
            Quaternion.identity,
            pivotInstanceW.transform);

        pivotInstanceW.transform.rotation = Quaternion.Euler(0f, 90f, 0f);

        pivotInstanceW.SetActive(false);


        GameObject pivotInstanceE = Instantiate(rotationPivot,pivotInstance.transform.position, Quaternion.identity, pivotInstance.transform);
        GameObject eR = Instantiate(rotateObject, new Vector3(pivotInstanceE.transform.position.x + radius,
                                              pivotInstanceE.transform.position.y,
                                              pivotInstanceE.transform.position.z) ,
            Quaternion.identity,
            pivotInstanceE.transform);

        pivotInstanceE.transform.rotation= Quaternion.Euler(0f,270f, 0f);

        pivotInstanceE.SetActive(false);

        yield return new WaitForSeconds(buildTime);

        pivotInstanceN.SetActive(true);

        yield return new WaitForSeconds(buildTime);

        pivotInstanceW.SetActive(true);

        yield return new WaitForSeconds(buildTime);

        pivotInstanceS.SetActive(true);

        yield return new WaitForSeconds(buildTime);

        pivotInstanceE.SetActive(true);


    }

    public void RebuildRotor()
    {
       //Particle effects would be nice
       Destroy(pivotInstance);
       pivotInstance = Instantiate(rotationPivot, new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), Quaternion.identity, transform); // or just create a transform that sits where we want on the player
       StartCoroutine(SetUpRotor());
    }

    

}
