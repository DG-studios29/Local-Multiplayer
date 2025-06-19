using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int damage;
    private GameObject shooter;
    public float speed = 20f;
    public static bool ChainReactionActive = false;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed;

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
            collision.gameObject.GetComponent<PlayerHealth>()?.TakeDamage(damage, gameObject);
            collision.gameObject.GetComponent<EnemyAI>()?.TakeDamage(damage, gameObject);

            if (ChainReactionActive)
            {
                Collider[] hits = Physics.OverlapSphere(hitPoint, 3f);
                foreach (var obj in hits)
                {
                    if (obj.gameObject == collision.gameObject) continue;
                    obj.GetComponent<PlayerHealth>()?.TakeDamage(damage / 2, gameObject);
                    obj.GetComponent<EnemyAI>()?.TakeDamage(damage / 2, gameObject);
                }

                Debug.Log("Chain Reaction triggered!");
            }

            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject, 2f);
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
