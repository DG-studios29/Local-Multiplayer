using UnityEngine;

public class RotateCollision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public ItemRotate rotorParent;
    
    //private int projectileID;

    private float maxProjectileHP = 100;
    [SerializeField]private float projectileHP;

    [SerializeField] private GameObject explosionFX;
    [SerializeField] private GameObject clashFX;


    void Start()
    {
        rotorParent = GetComponentInParent<ItemRotate>();
        projectileHP = maxProjectileHP;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collision");

    }

    public void OnRotationCollision()
    {
        projectileHP -= 30;

        if (clashFX != null)
        {
            GameObject collisionFX = GameObject.Instantiate(clashFX, transform.position, Quaternion.identity);
            Destroy(collisionFX, 2f);
        }

        if (projectileHP < 0)
        {
            projectileHP = 0;

            
            
            GameObject explodeFX = GameObject.Instantiate(explosionFX, transform.position, Quaternion.identity);

            Destroy(explodeFX, 2f);

            DestroyProjectile();
        }

    }

    public void DestroyProjectile() 
    {
        //tell something to the rotor parent, like ID of destroyed projectile
        Destroy(gameObject);

    }
}
