using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    [HideInInspector]public GameObject referenceMesh;
    public Material ghostMaterial;
    public float ghostLifetime = .5f;
    public float spawnInterval = .15f;

    private float spawnTimer;
    private Rigidbody playerRB;

    private void OnEnable()
    {
        if(referenceMesh != null) 
        playerRB = referenceMesh.GetComponent<Rigidbody>();
    }

    void Update()
    {
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            SpawnGhost();
            spawnTimer = 0f;
        }
    }

    void SpawnGhost()
    {
        if (referenceMesh == null) return;
        GameObject ghost = Instantiate(referenceMesh, referenceMesh.transform.position, referenceMesh.transform.rotation, null);
        ghost.transform.localScale = referenceMesh.transform.lossyScale;

        for(int i = 0; i<ghost.transform.childCount; i++)
        {
            if(ghost.transform.GetChild(i).name != "root")
            {
                Destroy(ghost.transform.GetChild(i).gameObject);
            }
        }

        ghost.tag = "Untagged";
        int layer = LayerMask.NameToLayer("Default");
        ghost.layer = layer;
        ghost.name = "ghost"+1;

        foreach (var rend in ghost.GetComponentsInChildren<Renderer>())
            if(ghostMaterial != null)
            rend.material = ghostMaterial;

        foreach (Component component in ghost.GetComponents<Component>())
        {
            if(!(component is Transform))
                Destroy(component);
        }

        if (playerRB != null)
        {
            if (playerRB.linearVelocity.magnitude < 0.1f)
            {
                Destroy(ghost);
            }
        }

        Destroy(ghost, ghostLifetime);
    }
}
