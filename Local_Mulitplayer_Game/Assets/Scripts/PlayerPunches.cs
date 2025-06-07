using UnityEngine;

public class PlayerPunches : MonoBehaviour
{

    private PlayerController playerController;
    private GameObject parentObject;
    private Animator animator;


    //Punching
    public bool isPunchR = false;
    [SerializeField] private float punchDamage;
    [SerializeField] private float unscaledMAXDamage;

    [SerializeField] private float punchRadius;
    [SerializeField] private float punchDistance;
    [SerializeField] private LayerMask playerMask;
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





    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        parentObject = playerController.gameObject;
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
    }

    // Update is called once per frame
    void Update()
    {
        lastPunchTimer += Time.deltaTime;

        ChargeUpdate();

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


        Debug.Log("Called Punch");


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
                    targetHealth.TakeDamage((int)punchDamage);

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
                    targetHealth.TakeDamage((int)niceDMG);
                }
                else
                {

                    //perform animation
                    targetControl = target.GetComponent<PlayerController>();
                    targetControl.Animator.SetTrigger("CriticalHit");

                    //trigger camera shake
                    

                    //apply forces
                    punchForce = distancePushed * 2f / timePushed;
                    Vector3 criticalVelocity = punchForce * hit.rigidbody.mass * (transform.forward + transform.up);
                    hit.rigidbody.AddForce(criticalVelocity, ForceMode.Impulse);

                    //show particle FX
                    GameObject hypeFX = GameObject.Instantiate(upperCutFX, transform.position + (transform.forward * 1.5f), Quaternion.identity);
                    Destroy(hypeFX, 3f);

                    //critical hit
                    float crit = unscaledMAXDamage;
                    targetHealth.TakeDamage((int)crit);

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
                        targetHealth.TakeDamage(1f);
                    }
                    else
                    {
                        targetHealth.TakeDamage(5f);
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
        Debug.Log("ChargingUp");
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


    private void OnDrawGizmos()
    {
        Vector3 GizmoPos = transform.position + new Vector3(0, 1, 0);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GizmoPos + transform.forward * punchDistance, punchRadius);
    }

}
