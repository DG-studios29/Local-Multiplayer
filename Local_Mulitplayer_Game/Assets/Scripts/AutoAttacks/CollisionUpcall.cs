using UnityEngine;

public class CollisionUpcall : MonoBehaviour
{

    [SerializeField]private RotateCollision rotateParent;
    [SerializeField]private GameObject parentPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotateParent = GetComponentInParent<RotateCollision>();
        parentPlayer = rotateParent.rotorParent.parentPlayer;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Army"))
        {
            var enemyCheck = other.gameObject.GetComponent<EnemyAI>();
            if(enemyCheck != null)
            {
                if(enemyCheck.enemyParent != parentPlayer)
                {
                    //take damage
                    Debug.Log("Hit an Enemy Army");

                    var dmg = rotateParent.rotorParent.Damage;
                    enemyCheck.TakeDamage(dmg);

                    rotateParent.OnRotationCollision();
                }
            }
        }
    }


}
