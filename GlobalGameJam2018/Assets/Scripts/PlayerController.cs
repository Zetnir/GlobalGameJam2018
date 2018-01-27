using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    const float MAX_DAMAGE_RECIEVE = 100;

    //tres utile pour gerer les inputs
    public int numController;

    public Rigidbody playerRB;
    public Vector3 playerMovement = Vector3.zero;
    public LayerMask groundMask;
    public Vector3 direction;

    //Variable for movement 
    public float speedRot = 10;
    public float speed = 5f;
    public Quaternion lastRotation;

    //Variable for jump
    /*public float jumpVelocity = 8f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public bool isGrounded = false;*/

    //Variable for punch
    public float punchCD = 0.25f;
    public float punchDuration = 0.5f;
    public bool isPunched = false;
    public bool canPunch = true;
    public float punchTimer;
    public float punchCDTimer;
    public bool isPunching = false;
    public Collider punchCol;


    //Var..iable for dash
    public bool isDashing = false;
    public float dashCD = 1f;
    public float dashForce = 7;
    public float dashTime;

    //Variable for grab
    /*public bool isGrabbing = false;
    public bool grabbedSomeone = false;
    public bool attemptGrabDistance = false;
    public bool canGrab = true;
    public bool isGrabbed = false;
    public float grabDistance;
    public float grabDuration;
    public float grabStartDistance;
    public float grabCD = 2f;
    public float grabMaxDistance = 7f;
    public float grabSpeed = 15;
    public float grabTime;
    public float grabbedTime;
    public float grabbedCD = 0.5f;
    public float actualDistance;
    public Vector3 grabStartPosition;
    public Collider grabCol;*/

    //Variable for the shield
    /*public Collider shieldCol;*/

    //Variable for fighting
    public float weakness = 50;
    public float damagesRecieve = 0;
    public bool isDead = false;
    public float weakChange = 12.5f;
    public bool isStun = false;
    public float stunDuration = 0.5f;
    public float powerJauge = 0f;
    public int stunCount = 0;
    public int stunStack_ = 0;
    public bool isBonus = false;
    public bool isMalus = false;
    public BallScript ball;
    public GameManager gameManager;
    // Use this for initialization
    void Start()
    {
        // est surement pas tres utile
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        playerRB = GetComponent<Rigidbody>();
        punchCol = transform.GetChild(0).GetComponent<Collider>();
        /*grabCol = transform.GetChild(2).GetComponent<Collider>();
        shieldCol = transform.GetChild(4).GetComponent<Collider>();*/
        // punchObject = transform.Find("Cube").GetComponent<GameObject>();
        groundMask = LayerMask.GetMask("Ground");

        //Input.
    }

    // Update is called once per frame
    void Update()
    {
        float h = gameManager.horizontal[numController];
        float v = gameManager.vertical[numController];
        bool jump = gameManager.jump[numController];
        bool punch = gameManager.fire1[numController];
        float dash = gameManager.triggers[numController];
        bool bonus = gameManager.fire2[numController];
        bool malus = gameManager.cancel[numController];

        if (!isStun)
        {
            if(!isPunching)
            {
                Move(h, v);
                Turning(h, v);
                LaunchDash(dash, h, v);
            }
            Attack(punch);
        }
        else
        {
            if(stunCount < (8 + 2*stunStack_))
            {
                if(jump)
                stunCount++;
            }
            else
            {
                stunCount = 0;
                isStun = false;
            }
        }

        ball = GameObject.Find("Sphere").GetComponent<BallScript>();
        powerJauge = Mathf.Clamp(powerJauge,0, 100);
        if(powerJauge >= 95)
        {
            //Power(bonus, malus);
        }
    }

    void FixedUpdate()
    {

        Death();
        weakness = Mathf.Clamp(weakness, 0, 125);
    }
    // Move the player around the scene.
    void Move(float h, float v)
    {
        // Set the movement vector based on the axis input.
        playerMovement.Set(h, 0f, v);

        // Normalise the movement vector and make it proportional to the speed per second.
        playerMovement = playerMovement.normalized * speed * Time.deltaTime;

        // Move the player to it's current position plus the movement.
        playerRB.MovePosition(transform.position + playerMovement);

    }

    void Power(bool bonus, bool malus)
    {
        powerJauge = 0f;
        if(bonus)
        {
            isBonus = true;
            StartCoroutine(PowerDuration(4f));

        }

        if (malus)
        {
            isMalus = true;
            gameManager.malusCount++;
            StartCoroutine(PowerDuration(10f));

        }

    }

    IEnumerator PowerDuration(float timeWait)
    {
        yield return new WaitForSeconds(timeWait);
        isBonus = false;
        isMalus = false;

    }
    public void Stun(int powerStack)
    {
        isStun = true;
        stunStack_ = powerStack;
        stunCount = 0;
        StartCoroutine(StunWait(powerStack * 1.5f + stunDuration));
    }

    IEnumerator StunWait(float timeWait)
    {
        yield return new WaitForSeconds(timeWait);
        if(isStun)
        isStun = false;
    }

    void Attack(bool punch)
    {
        if (punch && !isPunching)
        {
            punchCol.enabled = true;
            punchCol.GetComponent<MeshRenderer>().enabled = true;
            punchTimer = Time.time;
            isPunching = true;
            playerRB.constraints = RigidbodyConstraints.FreezeRotation;

        }

        if (Time.time - punchTimer > punchDuration)
        {
            punchCol.enabled = false;
            isPunching = false;
            punchCol.GetComponent<MeshRenderer>().enabled = false;
            playerRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

    }
    // Enable the player to jump and fall
    /*void Jump(bool jump)
    {
        if(jump && isGrounded)
        {
            playerRB.velocity = Vector3.up * jumpVelocity;
        }
        if(!isGrounded)
        {
            if (playerRB.velocity.y < 0)
            {
                playerRB.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (playerRB.velocity.y > 0)
            {
                playerRB.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }
    }*/
    // Turn the player to face the mouse cursor.
    void Turning(float h, float v)
    {
        if(h!=0 || v!=0)
        {
            playerRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

            float rot = Mathf.Atan2(v, -h) * Mathf.Rad2Deg + 90;
            Quaternion newRot = Quaternion.Euler(0, rot, 0);

            playerRB.rotation = Quaternion.Slerp(transform.rotation, newRot, Time.deltaTime * speedRot);
        }
        else
        {
            playerRB.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    /*void BasicAttack(bool punch, bool jump, bool special)
    {

        if(punch && !isPunching && canPunch && !jump && !special)
        {
            isPunching = true;
            punchTimer = punchDuration;
            canPunch = false;
            punchCol.enabled = true;
            punchCol.GetComponent<MeshRenderer>().enabled = true;
        }

        if(!canPunch)
        {
            if(punchTimer > 0)
            {
                punchTimer -= Time.deltaTime;
            }
            else if(isPunching && punchTimer <= 0)
            {
                isPunching = false;
                punchCol.enabled = false;
                punchCol.GetComponent<MeshRenderer>().enabled = false;
                punchTimer = punchCD;
            }
            else
            {
                canPunch = true;
                punchTimer = 0;
            }
        }

    }*/

    void LaunchDash(float dash, float h, float v)
    {
        if(dash == -1 && !isDashing)
        {
            isDashing = true;
            dashTime = dashCD;
            Vector3 direction = new Vector3(h,0f,v);
            direction = direction.normalized;
            playerRB.velocity = direction * dashForce;

        }
        
        if (isDashing)
        {
            if (dashTime > 0)
            {
                dashTime -= Time.deltaTime;
            }
            else
            {
                isDashing = false;
                dashTime = 0;
            }
        }
    }

    void Death()
    {
        if (isDead)
        {
            Destroy(gameObject);
        }

    }

    /*void LaunchGrab(bool grab)
    {
        actualDistance = (transform.position - grabCol.transform.position).magnitude;

        if (grab && !isGrabbing && canGrab)
        {
            isGrabbing = true;
            attemptGrabDistance = false;
            grabCol.transform.position = transform.position;
            canGrab = false;
            grabTime = grabCD;
            playerRB.constraints = RigidbodyConstraints.FreezeRotation;
        }

        if (grabDistance >= 0 && !attemptGrabDistance && !grabbedSomeone)
        {
                grabDistance = grabMaxDistance - actualDistance;
        }
        else if(actualDistance > grabStartDistance + 1f)
        {
            grabDistance = grabMaxDistance - actualDistance;
            attemptGrabDistance = true;
        }
        else
        {
            grabCol.transform.position = transform.position;
            actualDistance = grabStartDistance;
            isGrabbing = false;
            attemptGrabDistance = false;
            playerRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        }

        if (isGrabbing && !attemptGrabDistance && !grabbedSomeone)
        {
            grabCol.enabled = true;
            grabCol.GetComponent<MeshRenderer>().enabled = true;
            grabCol.transform.Translate( -grabCol.transform.forward * grabSpeed *  Time.deltaTime,Space.World);
        }
        else if(isGrabbing && (attemptGrabDistance || grabbedSomeone))
        {
            grabCol.transform.Translate(grabCol.transform.forward * grabSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            grabCol.enabled = false;
            grabCol.GetComponent<MeshRenderer>().enabled = false;
            grabbedSomeone = false;
        }

        if (!canGrab && !isGrabbing)
        {
            if (grabTime > 0)
            {
                grabTime -= Time.deltaTime;
            }
            else
            {
                canGrab = true;
                grabTime = 0;
            }
        }

        if (isGrabbed)
        {
            grabCol.transform.position = transform.position;
            grabCol.enabled = false;
            grabCol.GetComponent<MeshRenderer>().enabled = false;
            grabbedSomeone = false;
            canGrab = true;
            attemptGrabDistance = false;

            if (grabbedTime > 0)
            {
                grabbedTime -= Time.deltaTime;
            }
            else
            {
                isGrabbed = false;
                grabbedTime = 0;
            }
        }
        
    }*/

    /*void LaunchShield(float shield)
    {
        if(shield == 1)
        {
            shieldCol.enabled = true;
            shieldCol.GetComponent<MeshRenderer>().enabled = true;
        }
        else
        {
            shieldCol.enabled = false;
            shieldCol.GetComponent<MeshRenderer>().enabled = false;
        }
    }*/

    public void DamageCalculator(float damageDeal)
    {
        damagesRecieve += damageDeal;
        if (damagesRecieve >= MAX_DAMAGE_RECIEVE)
        {
            weakness += weakChange;
            damagesRecieve = 0;
        }
    }

}