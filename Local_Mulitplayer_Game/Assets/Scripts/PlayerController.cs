using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IPlayerEffect
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public bool isWalking = true;
    [SerializeField] private LayerMask objectsToCheckAgainst; //for collision detection
    public static bool ReverseControlsActive = false;
    public PlayerInput playerInput;

    #region Pickup Variables
    Coroutine speedCoroutine;

    //new changes
    private GameObject activeGhostTrail;
    private int originalSpeed = 10;
    private float currentSpeedBoostTimer = 0;
    private Image speedImage;
    public enum IsPlayer { PlayerOne, PlayerTwo }
    public IsPlayer isPlayer;

    #endregion

    //public bool isPunchR = false;

    private Animator animator;
    public Animator Animator => animator;
  

    private Rigidbody rb;
    private Vector2 movementInput;


    //Gonna store all this stuff in a Player Punches script after merge
    private PlayerPunches playerPunches;


    private void OnEnable()
    {
        StartCoroutine(ValidatePlayer());
        ArenaEventManager.OnArenaEventStart += HandleArenaEvent;
        ArenaEventManager.OnArenaEventEnd += HandleArenaEventEnd;
    }

    private IEnumerator ValidatePlayer()
    {
        yield return new WaitForSeconds(5f);
        if (gameObject.name == "Player 1") isPlayer = IsPlayer.PlayerOne;
        if (gameObject.name == "Player 2") isPlayer = IsPlayer.PlayerTwo;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();
        playerPunches = GetComponent<PlayerPunches>();

        ArenaEventManager.OnArenaEventStart += HandleArenaEvent;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();

        if (ReverseControlsActive)
            input *= -1f;

        movementInput = input;
    }





    public void OnPunch(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            playerPunches.ChargingCall(true);
        }

        if (context.canceled)
        {
            playerPunches.PunchCall();
            TutorialActionLinq(context);
            //playerPunches.AnimatorChargeClear();

        }

    }

    public void OnClear(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            AutoAttack auto = GetComponentInChildren<AutoAttack>();
            auto.TestClear();
        }
    }
    public void TutorialActionLinq(InputAction.CallbackContext context)
    {
        //Checking if its null
        if (!TutorialManager.instance) return;
        if (TutorialManager.instance.isTutorialActive)
        {
            TutorialManager.instance.CheckTutorialPerform(context.action.name);
        }
    }


    private void FixedUpdate()
    {
        Vector3 moveDirection = new Vector3(movementInput.x, 0, movementInput.y);
        isWalking = moveDirection.magnitude > 0.1f;

        if (isWalking && !CollidingWithObstacle())
        {
            // Smooth acceleration
            Vector3 targetVelocity = moveDirection.normalized * moveSpeed;
            Vector3 velocityChange = (targetVelocity - rb.linearVelocity);
            velocityChange.y = 0; // Don't change vertical velocity

            rb.AddForce(velocityChange, ForceMode.VelocityChange);

            // Smooth rotation
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);

            animator.SetBool("isWalking", true);
        }
        else
        {
            // Smooth stop
            Vector3 reducedVelocity = rb.linearVelocity * 0.9f;
            reducedVelocity.y = rb.linearVelocity.y; 
            rb.linearVelocity = reducedVelocity;

            animator.SetBool("isWalking", false);
        }

        //attempts to prevent players form getting stuck on doorways or walls
        if (CollidingWithObstacle())
        {
            var lookDir = transform.position - transform.forward;
            Quaternion targetRotation = Quaternion.LookRotation(-lookDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * 10f);
        }
    }

    private void HandleArenaEvent(ArenaEventSO evt)
    {
        if (evt.triggerReverseControls)
            ReverseControlsActive = true;
    }

    private void HandleArenaEventEnd(ArenaEventSO evt)
    {
        if (evt.triggerReverseControls)
            ReverseControlsActive = false;

    }

    public void SwitchInputTutorial()
    {
        playerInput.SwitchCurrentActionMap("Tutorial");
    }

    public void SwitchInputUI()
    {
        playerInput.SwitchCurrentActionMap("UI");
    }

    public void SwitchInputPlayer()
    {
        playerInput.SwitchCurrentActionMap("Player");
    }

  

    private void Update()
    {
        //// Testing player Health 
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    GetComponent<PlayerHealth>().TakeDamage(10);
        //}

        //if (Input.GetKeyDown(KeyCode.J))
        //{
        //    GetComponent<PlayerHealth>().Heal(10);
        //}
    }


    /*   private void OnDrawGizmos()
       {
           Vector3 GizmoPos = transform.position + new Vector3(0, 1, 0);

           Gizmos.color = Color.yellow;
           Gizmos.DrawWireSphere(GizmoPos + transform.forward * punchDistance,punchRadius);
       }*/

    private bool CollidingWithObstacle()
    {
        return Physics.Raycast(transform.position + new Vector3(0, .7f, 0), transform.forward, out RaycastHit hitInfo, .1f, objectsToCheckAgainst) ? true : false;
    }

    #region Interface / Pickups

    public void ActivateSpeedBoost(float duration, float speedMultiplier, GameObject trailEffect)
    {
        if (speedCoroutine != null)
        {
            StopCoroutine(speedCoroutine);
            speedCoroutine = null;
        }

        moveSpeed += moveSpeed == originalSpeed ? speedMultiplier : 0;

        if (activeGhostTrail != null)
            Destroy(activeGhostTrail);


        activeGhostTrail = Instantiate(trailEffect, transform);
        activeGhostTrail.transform.localPosition = new Vector3(0, 0, 0);

        GhostTrail ghostTrail = activeGhostTrail.GetComponent<GhostTrail>();

        if (ghostTrail != null)
        {
            ghostTrail.referenceMesh = gameObject;
        }

        speedCoroutine = StartCoroutine(SpeedBoostEffect(duration));

        switch (isPlayer)
        {
            case IsPlayer.PlayerOne:
                GameManager.Instance.playerOnePowerUps[2].alpha = 1f;
                speedImage = GameManager.Instance.playerOnePowerUps[2].gameObject.GetComponent<Image>();
                break;

            case IsPlayer.PlayerTwo:
                GameManager.Instance.playerTwoPowerUps[2].alpha = 1f;
                speedImage = GameManager.Instance.playerTwoPowerUps[2].gameObject.GetComponent<Image>();
                break;
        }
    }

    private IEnumerator SpeedBoostEffect(float duration)
    {
        yield return StartCoroutine(CountHelper(duration));

        moveSpeed = originalSpeed;
        currentSpeedBoostTimer = 0;

        if (activeGhostTrail != null)
        {
            Destroy(activeGhostTrail);
        }

        switch (isPlayer)
        {
            case IsPlayer.PlayerOne:
                GameManager.Instance.playerOnePowerUps[2].alpha = 0.1f;
                break;

            case IsPlayer.PlayerTwo:
                GameManager.Instance.playerTwoPowerUps[2].alpha = 0.1f;
                break;
        }
    }

    private IEnumerator CountHelper(float dur)
    {
        currentSpeedBoostTimer = dur;
        while (currentSpeedBoostTimer > 0)
        {
            currentSpeedBoostTimer -= Time.deltaTime;

            if (speedImage != null)
            {
                speedImage.fillAmount = currentSpeedBoostTimer / dur;
            }

            yield return null;
        }
    }

    public void ActivateShield(float duration, GameObject shield)
    {
        //
    }

    public void GiveHealth(float health)
    {
        //
    }

    public void RestoreOrbs()
    {
        //
    }

    public void ResetAbilityCooldownTimer(int cooldown)
    {

    }
    #endregion

    private void OnDestroy()
    {
        ArenaEventManager.OnArenaEventStart -= HandleArenaEvent;
        ArenaEventManager.OnArenaEventEnd -= HandleArenaEventEnd; 
    }
}