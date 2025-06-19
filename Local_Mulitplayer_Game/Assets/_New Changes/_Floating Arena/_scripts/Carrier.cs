using UnityEngine;

public class Carrier : MonoBehaviour
{
    #region Methods-Only
    public void ReleasePlayer()
    {
        Transform player = transform.GetChild(0);
        player.SetParent(null);
        player.GetComponent<Rigidbody>().useGravity = true;
        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        playerHealth.TakeDamage(50,gameObject);
    }

    public void DestroyObject()
    {
        Destroy(transform.parent.gameObject);
    }
    #endregion
}
