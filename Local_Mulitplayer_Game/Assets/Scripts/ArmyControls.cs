using System.Collections;
using UnityEngine;

public class ArmyControls : MonoBehaviour
{
    public float health = 100f;
    public float damage = 30f;
    private Animator animator;
    public float damagePerSecond = 10f;
    public Collider armyCollider;

    private bool isDead = false;

    [SerializeField]private Material hurtMaterial;
    [SerializeField] private float hurtTime = 0.25f;
    bool alreadyHurting = false;
    private Material baseMaterial;
    private MeshRenderer[] partyMeshRenderers;


    void Start()
    {
        animator = GetComponent<Animator>();

        partyMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        baseMaterial = partyMeshRenderers[0].material;
    }
    
    private void Update()
    {
        //TakeDamage(damagePerSecond * Time.deltaTime);
        //Debug.Log($"Health is now {health}.");
    }

    public void AnimateAttack()
    {
        animator.SetTrigger("Attack");
    }

    public void AnimateHealing()
    {
        animator.SetBool("isHealing",true);
    }

    public void AnimateStopHealing()
    {
        animator.SetBool("isHealing", false);
    }

    public void ShowDamage()
    {
        //start like a coroutine that changes materials and what not
        if(!alreadyHurting)
        StartCoroutine(MatChange());
    }

    IEnumerator MatChange()
    {
        alreadyHurting = true;
        foreach(var party in partyMeshRenderers)
        {
            party.material = hurtMaterial;
        }

        yield return new WaitForSeconds(hurtTime);

        foreach (var party in partyMeshRenderers)
        {
            party.material = baseMaterial;
        }

        alreadyHurting=false;
        
    }

    // Take damage
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Health: {health}");

        if (health <= 0)
        {
            onDeath();
        }
    }


    public void onDeath()
    {
        if (!isDead)
        {
            isDead = true;
            animator.SetBool("isDead", true);
            Debug.Log($"{gameObject.name} has died.");

            armyCollider.enabled = false;
            StartCoroutine(DestroyAfterAnimation());
           
        }
    }

    // Delay death to finish the animation 
    private IEnumerator DestroyAfterAnimation()
    {
        AnimatorStateInfo dead = animator.GetCurrentAnimatorStateInfo(1);
        yield return new WaitForSeconds(dead.length);

        Destroy(gameObject,10f);
    }
}
