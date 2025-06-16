using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    private GameObject shooter;
    public static bool ChainReactionActive = false;

    private void Start()
    {
        ArenaEventManager.OnArenaEventStart += HandleArenaEvent;

    }
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
            return;

        Vector3 hitPoint = collision.contacts[0].point;

        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(damage, shooter);
            collision.gameObject.GetComponent<EnemyAI>()?.TakeDamage(damage, shooter);


            if (ChainReactionActive)
            {
                Collider[] hits = Physics.OverlapSphere(hitPoint, 3f);
                foreach (var obj in hits)
                {
                    if (obj.gameObject == collision.gameObject) continue;
                    obj.GetComponent<PlayerHealth>()?.TakeDamage(damage / 2, shooter);
                    obj.GetComponent<EnemyAI>()?.TakeDamage(damage / 2, shooter);

                }

                // Optional: Add VFX or explosion sound here
                Debug.Log("Chain Reaction triggered!");
            }

            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    private void HandleArenaEvent(ArenaEventSO evt)
    {
        ChainReactionActive = evt.triggerChainReaction;
    }

    private void OnDestroy()
    {
        ArenaEventManager.OnArenaEventStart -= HandleArenaEvent;
    }

}
