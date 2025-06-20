using System.Collections;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    #region Custom Vars
    public Transform winner;
    public Camera focusCamera;
    public float camSpeed = 1f;
    public float stoppingDistance = 10f;
    public float yVar = 2f;

    #endregion

    #region Built-In Methods

    private void OnEnable()
    {
        Camera.main.gameObject.SetActive(false);
        focusCamera.transform.position = new Vector3(0f, 60f, 0f);

        StartCoroutine(Wait());
    }

    // private void OnDisable()
    // {
    //     focusCamera.gameObject.SetActive(false);
    //     Camera.main.gameObject.SetActive(true);
    // }

    private IEnumerator Wait()
    {
        yield return new WaitForSeconds(.1f);
        winner = GameManager.Instance.winner.transform;
    }

    private void LateUpdate()
    {
        if(winner!= null)
        {
            FocusOnWinner();
        }
    }

    #endregion

    #region Custom Methods

    public void FocusOnWinner()
    {
        var rightPos = new Vector3 (winner.position.x, yVar, winner.position.z);
        var targetPos = rightPos + winner.forward * stoppingDistance;

        if(Vector3.Distance(transform.position, targetPos)> 0.05f)
            transform.position = Vector3.Lerp(transform.position, targetPos, camSpeed * Time.deltaTime);
        else
            transform.position = targetPos;
        transform.LookAt(winner.position, Vector3.up);
    }

    #endregion
}