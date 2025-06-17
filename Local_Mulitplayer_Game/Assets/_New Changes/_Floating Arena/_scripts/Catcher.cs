using UnityEngine;

public class Catcher : MonoBehaviour
{
    #region Custom Vars
    public GameObject carPrefab;

    private Vector3 center = new Vector3(35f, 0f, 35f);
    #endregion

    #region Custom Methods

    private void OnTriggerEnter(Collider col)
    {
        if (col.CompareTag("Player"))
        {
            GameObject player = col.gameObject;
            Rigidbody playerRB = player.GetComponent<Rigidbody>();
            playerRB.useGravity = false;

            Vector3 direction = player.transform.position - center;
            direction.y = 0f;

            float correctAngle = Vector3.SignedAngle(-Vector3.forward, direction, Vector3.up);

            GameObject lift = Instantiate(carPrefab, center, Quaternion.Euler(0, correctAngle, 0));
            Transform liftPlatform = lift.transform.GetChild(0); 
            player.transform.SetParent(liftPlatform, true);
        }
    }

    #endregion
}
