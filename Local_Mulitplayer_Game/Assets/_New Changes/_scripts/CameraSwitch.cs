using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    #region Custom Vars

    public Camera focusCamera;
    public float camSpeed = 1f;
    private float stoppingDistance = 20f;

    #endregion

    #region Built-In Methods

    private void OnEnable()
    {
        
        Camera.main.gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if(focusCamera!= null)
        {
            
        }
    }

    #endregion

    #region Custom Methods

    public void FocusOnWinner(Transform player)
    {
        Vector3 newCamPos = Vector3.Lerp(focusCamera.transform.position, player.position, camSpeed * Time.smoothDeltaTime);
        focusCamera.transform.position = newCamPos;
        focusCamera.transform.LookAt(newCamPos);

        if (Vector3.Distance(focusCamera.transform.position, newCamPos) < stoppingDistance)
            camSpeed = 0;
        else camSpeed = 1;
    }

    #endregion
}
