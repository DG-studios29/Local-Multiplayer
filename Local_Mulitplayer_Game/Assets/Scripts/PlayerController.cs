using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IPlayerEffect
{
    [Header("Movement Settings")] public float moveSpeed = 10f;
    public bool isWalking = true;
    [SerializeField] private LayerMask objectsToCheckAgainst; //for collision detection
    public static bool ReverseControlsActive = false;


    #region Pickup Variables

    bool hasTrail = false;
    Coroutine speedCoroutine;

    public enum IsPlayer
    {
        PlayerOne,
        PlayerTwo
    }

    public IsPlayer isPlayer;

    #endregion

    //public bool isPunchR = false;

    private Animator animator;
    public Animator Animator => animator;


    private Rigidbody rb;
    private Vector2 movementInput;


    //Player Punch inputs
    private PlayerPunches playerPunches;

    //Player Input 
    private PlayerInput playerInput;

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
        Debug.Log("[Player] Player prefab instantiated!");
        ArenaEventManager.OnArenaEventStart += HandleArenaEvent;
        

        animator = GetComponent<Animator>();

        playerPunches = GetComponent<PlayerPunches>();

        playerInput = GetComponent<PlayerInput>();
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

    void TutorialActionLinq(InputAction.CallbackContext context)
    {
        //Checking if its null
        if (!TutorialManager.instance) return;
        if (TutorialManager.instance.isTutorialActive)
        {
            TutorialManager.instance.CheckTutorialPerform(context.action.name);
        }
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
            //playerPunches.AnimatorChargeClear();

            TutorialActionLinq(context);

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
        return Physics.Raycast(transform.position + new Vector3(0, .7f, 0), transform.forward, out RaycastHit hitInfo, .5f, objectsToCheckAgainst) ? true : false;
    }

    #region Interface / Pickups

    public void ActivateSpeedBoost(float duration, float speedMultiplier, GameObject trailEffect)
    {
        moveSpeed += speedMultiplier;

        if (!hasTrail)
        {
            trailEffect = Instantiate(trailEffect);
            hasTrail = true;
        }

        trailEffect.transform.parent = transform;
        trailEffect.transform.localPosition = new Vector3(0, .01f, 0);
        if (speedCoroutine != null) StopCoroutine(speedCoroutine);
        speedCoroutine = StartCoroutine(SpeedBoostEffect(duration, trailEffect));

        switch (isPlayer)
        {
            case IsPlayer.PlayerOne:
                GameManager.Instance.playerOnePowerUps[2].alpha = 1f;
                break;

            case IsPlayer.PlayerTwo:
                GameManager.Instance.playerTwoPowerUps[2].alpha = 1f;
                break;
        }
    }

    private IEnumerator SpeedBoostEffect(float duration, GameObject trail)
    {
        yield return StartCoroutine(CountHelper(duration));

        moveSpeed = 10f;

        if(trail!=null)
        {
            Destroy(trail);
        }

        hasTrail = false;

        switch (isPlayer)
        {
            case IsPlayer.PlayerOne:
                GameManager.Instance.playerOnePowerUps[2].alpha = 0.4f;
                break;                                           
                                                                 
            case IsPlayer.PlayerTwo:                             
                GameManager.Instance.playerTwoPowerUps[2].alpha = 0.4f;
                break;
        }
    }

    private IEnumerator CountHelper(float dur)
    {
        float t = 0;
        while (t < dur)
        {
            t += Time.deltaTime;
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

    public void RefillAbilityBar(float energy)
    {
        //
    }
    #endregion

    private void OnDestroy()
    {
        ArenaEventManager.OnArenaEventStart -= HandleArenaEvent;
        ArenaEventManager.OnArenaEventEnd -= HandleArenaEventEnd; 
    }

    public void ResetAbilityCooldownTimer(int cooldown)
    {
       
    }

    public void RefillAbilityBar()
    {
       
    }
}