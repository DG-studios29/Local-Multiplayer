using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    private GameObject shooter;

    
    public void Initialize(GameObject owner, int damageAmount)
    {
        shooter = owner;
        damage = damageAmount; 
    }

    public void SetShooter(GameObject owner)
    {
        shooter = owner;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == shooter)
            return; // Prevent self-damage

        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(damage);
            collision.gameObject.GetComponent<EnemyAI>()?.TakeDamage(damage);
            Destroy(gameObject); 
        }
        else
        {
            Destroy(gameObject, 5f); 
        }
    }
}
