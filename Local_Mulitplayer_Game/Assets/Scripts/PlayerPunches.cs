using UnityEngine;

public class PlayerPunches : MonoBehaviour
{

    private PlayerController playerController;
    private GameObject parentObject;
    private Animator animator;

    //Rigidbody and other things
    private Rigidbody rb;
    [SerializeField]private Transform groundCheck;
    
    [SerializeField]private float gravityScale = 1.96f;
    private float globalGravity = -9.81f;
    
    //Projectile Motion 
    [SerializeField]private float launchAngle; //will say in inspector
    private float tanAlpha, cosAlpha,sinAlpha;
    [SerializeField]private float rangeZ;  //will say in inspector
    private float Uz, Uy, Uo;
    private float gravity;
    [SerializeField]private float heightY;
    private float heightMax;
    private float timeTaken;
    private Vector3 initialVelocity;
    private Vector3 globalVelocity;
    
    

    //Punching
    public bool isPunchR = false;
    [SerializeField] private float punchDamage;
    [SerializeField] private float unscaledMAXDamage;
    public static bool OnlyPunchesActive = false;

    [SerializeField] private float punchRadius;
    [SerializeField] private float punchDistance;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask groundMask;
    Vector3 punchPosition;
    private RaycastHit hit;

    //Punch Force
    private float punchForce;
    [SerializeField] private float timePushed;
    [SerializeField] private float distancePushed;

    //Punch Control - Anti-Spam cooldown
    private float punchCooldown = 0.25f; //animation time
    private float lastPunchTimer = 0.25f;

    //Punch Control - Critical-Hit Holding
    private bool chargeHolding = false;
    private float chargeHoldTimer = 0f;
    [SerializeField] private float maxChargeTime = 2.75f;
    private float chargeVal;
    //bool resetAnimation = false;
    private float dmgCalc;

    [SerializeField] private GameObject upperCutFX;

    // Audio for punches.
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip punchSound;
    [SerializeField] private AudioClip hitSound;
    //public static bool OnlyPunchesActive { get; set; }


    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        parentObject = playerController.gameObject;
        ArenaEventManager.OnArenaEventStart += HandleArenaEvent;
        
        rb = GetComponent<Rigidbody>();
    }


    void Start()
    {
        animator = GetComponent<Animator>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("AudioSource is missing on " + gameObject.name);
            }
        }
        
        InitializeVariables();
    }

    // Update is called once per frame
    void Update()
    {
        lastPunchTimer += Time.deltaTime;

        ChargeUpdate();
        
    }

    private void FixedUpdate()
    {
        Vector3 appliedGravity = Vector3.up * (globalGravity * gravityScale * gravityScale);
        rb.AddForce(appliedGravity, ForceMode.Acceleration);
    }


    private void InitializeVariables()
    {
        gravity = Physics.gravity.y;
        tanAlpha = Mathf.Tan(launchAngle * Mathf.Deg2Rad);
        cosAlpha = Mathf.Cos(launchAngle * Mathf.Deg2Rad);
        sinAlpha = Mathf.Sin(launchAngle * Mathf.Deg2Rad);


        Uz = Mathf.Sqrt((gravity*rangeZ * rangeZ)/(2*(heightY - rangeZ*tanAlpha)));
        Uy = tanAlpha * Uz;

        Uo = Mathf.Sqrt((Uz * Uz) + (Uy * Uy));

        timeTaken = -(Uo * sinAlpha * 2 / gravity);

        //Uo = -(timeTaken * gravity / sinAlpha * 2);
        //rangeZ = Uo * cosAlpha * timeTaken;

        //Uo = rangeZ / (timeTaken * cosAlpha);

        Uz = Uo * cosAlpha;
        Uy = Uo * sinAlpha;

       
        initialVelocity = new Vector3(0, Uy , Uz) * gravityScale;
    }
    
    
    
    private bool IsGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.55f,  groundMask);
    }
    
    
    private void HandleArenaEvent(ArenaEventSO evt)
    {
        OnlyPunchesActive = evt.triggerOnlyPunches;
    }

    public void PunchCall()
    {
        if (lastPunchTimer < punchCooldown)
        {
            //we wont punch if cooldown has not passed

            return;
        }

        //saves the time the charge was held for
        //ChargeSavedPower(); 
        chargeHolding = false;
        chargeHoldTimer = 0f;

        lastPunchTimer = 0;


        //AnimatorChargeClear();


        //Debug.Log("Called Punch");


        //ConfigureClip
        TogglePunch();


        animator.SetTrigger("Punch");

        if (audioSource != null && punchSound != null)
        {
            audioSource.volume = 0.1f;
            audioSource.PlayOneShot(punchSound);
        }



        punchPosition = transform.position + new Vector3(0, 1, 0);

        if (Physics.SphereCast(punchPosition, punchRadius, transform.forward, out hit, 3f, playerMask))
        {
            if (hit.collider.gameObject.CompareTag("Player"))
            {
                //recognize the player that got punched
                GameObject target = hit.collider.gameObject;
                PlayerHealth targetHealth = target.GetComponent<PlayerHealth>();
                PlayerController targetControl = target.GetComponent<PlayerController>();



                //determine damage based on charge
                if (chargeVal <= 0.3)
                {

                    //perform animation 
                    targetControl.Animator.SetTrigger("Hit");

                    //apply forces
                    punchForce = distancePushed / timePushed;
                    Vector3 nVelocity = punchForce * hit.rigidbody.mass * transform.forward;
                    hit.rigidbody.AddForce(nVelocity, ForceMode.Impulse);

                    //normal punch
                    targetHealth.TakeDamage((int)punchDamage, gameObject);

                    //chargeVal = 0;

                    if (audioSource != null && hitSound != null)
                    {
                        audioSource.PlayOneShot(hitSound);
                    }


                }
                else if (chargeVal > 0.3 && chargeVal <= 0.8)
                {
                    //perform animation 
                    targetControl.Animator.SetTrigger("Hit");

                    //apply forces
                    punchForce = distancePushed / timePushed;
                    Vector3 nVelocity = punchForce * hit.rigidbody.mass * transform.forward;
                    hit.rigidbody.AddForce(nVelocity, ForceMode.Impulse);

                    //nice damage
                    if (chargeVal > 0.6) chargeVal = 0.5f;
                    float niceDMG = unscaledMAXDamage * chargeVal;
                    targetHealth.TakeDamage((int)niceDMG, gameObject);
                }
                else
                {

                    //perform animation
                    targetControl = target.GetComponent<PlayerController>();
                    targetControl.Animator.SetTrigger("CriticalHit");

                    //trigger camera shake
                    

                    //apply forces
                    //punchForce = distancePushed * 2f / timePushed;
                    //Vector3 criticalVelocity = punchForce * hit.rigidbody.mass * (transform.forward + transform.up);
                    
                    globalVelocity = hit.transform.TransformDirection(initialVelocity);
                    //hit.rigidbody.AddForce(criticalVelocity, ForceMode.Impulse);
                    
                    hit.rigidbody.linearVelocity = globalVelocity;

                    //show particle FX
                    GameObject hypeFX = GameObject.Instantiate(upperCutFX, transform.position + (transform.forward * 1.5f), Quaternion.identity);
                    Destroy(hypeFX, 3f);

                    //critical hit
                    float crit = unscaledMAXDamage;
                    targetHealth.TakeDamage((int)crit, gameObject);

                }



            }

            else if (hit.collider.gameObject.CompareTag("Army"))
            {
                //recognize the player that got punched
                GameObject target = hit.collider.gameObject;
                EnemyAI targetHealth = target.GetComponent<EnemyAI>();

                if (targetHealth.enemyParent != parentObject)
                {
                    //Dont punch parents

                    //punches on armies are ineffective, save for crits
                    if (chargeVal <= 0.8f)
                    {
                        targetHealth.TakeDamage(1f, gameObject);
                    }
                    else
                    {
                        targetHealth.TakeDamage(5f, gameObject);
                    }
                }
            }

        }

        //resetAnimation = true;

        //AnimatorChargeClear();

    }



    void TogglePunch()
    {
        if (!isPunchR)
        {
            animator.SetBool("isPunchR", false);
            isPunchR = true;
        }
        else
        {
            animator.SetBool("isPunchR", true);
            isPunchR = false;
        }
    }


    public void ChargingCall(bool meleeBtnStatus)
    {
        chargeHolding = meleeBtnStatus;
        //ChargeSavedPower();

    }




    private void ChargeUpdate()
    {
        if (chargeHolding && (lastPunchTimer > punchCooldown))
        {
            chargeHoldTimer += Time.deltaTime;


            ChargeSavedPower();

        }
        else
        {
            //chargeHoldTimer = 0;
            AnimatorChargeClear();
            //resetAnimation = false;
        }

    }

    private void ChargeSavedPower()
    {
        //Debug.Log("ChargingUp");
        chargeVal = chargeHoldTimer / maxChargeTime;
        if (chargeVal > 1) chargeVal = 1;
        dmgCalc = chargeVal;

        animator.SetFloat("Charge", chargeVal, 0.05f, Time.deltaTime);


    }

    public void AnimatorChargeClear()
    {
        //Debug.Log("Clear");
        dmgCalc = chargeVal;
        chargeVal = 0;
        animator.SetFloat("Charge", chargeVal, 0.05f, Time.deltaTime);
    }

    private void OnDestroy()
    {
        ArenaEventManager.OnArenaEventStart -= HandleArenaEvent;
    }

    private void OnDrawGizmos()
    {
        Vector3 GizmoPos = transform.position + new Vector3(0, 1, 0);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GizmoPos + transform.forward * punchDistance, punchRadius);
    }

}
