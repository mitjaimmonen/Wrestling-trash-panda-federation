using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Player : MonoBehaviour
{

    public int playerNumber;
    public int meshNumber;
    public Playerdata playerData;

    [Tooltip("List of player skins (prefabs). Player chooses which one gets instantiated at main menu joining.")]
    public GameObject[] modelPrefabs = new GameObject[4];

    public Transform[] spawnPoints;
    public int maxHealth;
    public CapsuleCollider playerCollider;
    public SphereCollider playerTrigger;
    public GameObject takeDamageParticles;


    [FMODUnity.EventRef] public string damageSound;
    [FMODUnity.EventRef] public string deathSound;
    [FMODUnity.EventRef] public string getKnockedOutBitchSound;

    Animator anim;
    Health health;
    Rigidbody rb;



    float oldSpeed;
    public float maxSpeed;
    public float movementForce;
    [Range(0.01f, 0.99f), Tooltip("Deadzone for joystick movement (Left stick).")]
    public float movementDeadzone = 0.05f;
    [Range(0.01f, 0.99f), Tooltip("Deadzone for joystick rotation (Right stick).")]
    public float rotationDeadzone = 0.05f;
    public float jumpForce;
    public LayerMask groundLayerMask;
    public LayerMask enemyLayerMask;



    #region Actions data
        public int damage;
        public float chargeTime = 0.0f;
        public float hitTime = 0.1f;
        public float blockImpactForce = 5f;
        public float blockImpactStunTime = 1f;
        float blockPushTimer; //Counts when blocking enemy push
        float hitInputTimer; //Gives delay before deciding between hit and block
        bool hitInput;

        public bool isGrabbed = false;
        public bool isStunned = false;
        public bool isPushed = false;

        bool isGrounded = true;
        bool weaponCharging = false;
        bool weaponCharged = false;
        bool isGrabbing = false;
        bool isPushing = false;
        bool pushingEnemy = false;
        bool isBlocking = false;
        bool isHitting = false;
        bool leftHand = false;
        public Weapon currentWeapon;
        Player lastEnemyContact;


    #endregion



    float moveAxisV; //Stores input values 0-1
    float moveAxisH;
    float rotAxisV;
    float rotAxisH;
    float distToGround, distToFace;
    float globalGravity = 15f; //Change to affect falling speed
    float stunTime;
    float groundTimer = 0;
    bool allowGrounded = true;

    bool destroying;

    //public Player(Playerdata _playerData)
    //{
    //    playerData = _playerData;

    //    playerNumber = playerData.playerIndex;
    //    meshNumber = playerData.meshNumber;

    //}

    bool IsPushingEnemy()
    {
        // pushingEnemy = Physics.Raycast(transform.position, transform.forward, distToFace + 0.2f, enemyLayerMask);
        // pushingEnemy = Physics.CheckSphere(transform.position+ (transform.forward*(distToFace + 0.4f)), 0.3f, enemyLayerMask);
        RaycastHit hit;
        pushingEnemy = Physics.SphereCast(transform.position+ (transform.forward*(distToFace + 0.5f)), 0.5f, transform.forward, out hit, 0.1f, enemyLayerMask);
        if (pushingEnemy)
        {
            lastEnemyContact = hit.collider.gameObject.GetComponentInParent<Player>();
            if (lastEnemyContact)
            {
                lastEnemyContact.GetPushed(true, this);
            }
        }
        else if (lastEnemyContact)
        {
            lastEnemyContact.GetPushed(false, this);
        }
        return pushingEnemy;
    }

    void GetPushed(bool pushState, Player enemy)
    {
        if (isPushed && !pushState && lastEnemyContact != null && enemy != lastEnemyContact)
            return;
        lastEnemyContact = enemy;
        isPushed = pushState;
        if (isPushed)
        {
            transform.forward = -enemy.transform.forward;
            if (!isBlocking)
                rb.velocity = enemy.rb.velocity.normalized * (maxSpeed*0.75f);
            else
                rb.velocity = enemy.rb.velocity.normalized * (maxSpeed*0.25f);

        }
    }

    // void OnTriggerStay(Collider col)
    // {
    //     Player enemy = col.gameObject.GetComponentInParent<Player>();
    //     if(enemy && enemy != this)
    //     {
    //         Debug.Log("Collision");
    //         maxSpeed = oldSpeed * 0.0f;
    //     }
    // }
    // void OnTriggerExit(Collider col)
    // {
    //     Debug.Log("Collision exit");
    //     maxSpeed = oldSpeed;
    // }

    private void Awake()
    {
        health = new Health(maxHealth);
        rb = GetComponent<Rigidbody>();
        Physics.gravity = -Vector3.up*globalGravity;
        distToGround = playerCollider.bounds.extents.y;
        distToFace = playerCollider.bounds.extents.z;
        anim = GetComponentInChildren<Animator>();     
        oldSpeed = maxSpeed;
    }
    void FixedUpdate()
    {
        isGrounded = allowGrounded ? Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f, groundLayerMask) : false;
        anim.SetBool("isGrounded", isGrounded);
        if (!isGrounded)
        {
            EndPush();
            EndGrab();
        }
        
    }
    void Update()
    {
        anim.SetBool("hasWeapon", currentWeapon!=null);
        if (isPushed && isBlocking)
        {
            blockPushTimer += Time.deltaTime;
            if (blockPushTimer > 1f)
            {
                //Create impact against pushing enemy
                lastEnemyContact.isGrounded = false;
                lastEnemyContact.allowGrounded = false;
                lastEnemyContact.rb.velocity = (transform.forward*2 + Vector3.up*0.25f)*blockImpactForce;
                lastEnemyContact.AddBuff(true, false, blockImpactStunTime);
                Debug.Log(lastEnemyContact.isPushing + ", " + lastEnemyContact.isGrounded + ", " + lastEnemyContact.rb.velocity);
                isPushed = false;
            }
        }
        else
            blockPushTimer = 0;

        if (!allowGrounded)
        {
            groundTimer += Time.deltaTime;
            if (groundTimer > 0.35f)
            {
                allowGrounded = true;
                groundTimer = 0;
            }
        }

        if (hitInput)
            hitInputTimer += Time.deltaTime;
        else
            hitInputTimer = 0;
    }

    public void TransportToStart()
    {
        transform.position = spawnPoints[playerNumber].position;
        transform.rotation = spawnPoints[playerNumber].rotation;
    }

    public void HandleInput(GamePadState state, GamePadState prevState)
    {
        if (health.isAlive())
        {
            if (!isStunned)
            {
                if (isGrabbed || isPushed)
                {
                    HandleActions(state, prevState);
                }
                else
                {
                    HandleRotating(state);
                    HandleActions(state, prevState);

                }

            }

            HandleMenuInputs(state, prevState);

        }
    }

    public void HandleFixedInput(GamePadState state, GamePadState prevState)
    {
        if (health.isAlive())
        {
            HandleMove(state);
        }
    }

    void HandleMove(GamePadState state)
    {
        
        moveAxisH = state.ThumbSticks.Left.X;
        moveAxisV = state.ThumbSticks.Left.Y;
        Vector2 input = new Vector2(moveAxisH, moveAxisV);
        Vector2 newVelocity = Vector2.zero;
        // animControl.SetVerticalMagnitude(moveAxisH);

        if (!isPushing )
        {
            if (input.magnitude > movementDeadzone && !isStunned)
            {
                // input.x = input.x != 0 && input.x < 0 ? input.x + movementDeadzone : input.x;
                // input.x = input.x != 0 && input.x > 0 ? input.x - movementDeadzone : input.x;
                // input.z = input.z != 0 && input.z < 0 ? input.z + movementDeadzone : input.z;
                // input.z = input.z != 0 && input.z > 0 ? input.z - movementDeadzone : input.z;
                newVelocity = input * movementForce * Time.fixedDeltaTime * 100f;

                bool colliding = false;
                Vector3 direction = new Vector3(newVelocity.x,0,newVelocity.y).normalized;
                RaycastHit[] hits = Physics.SphereCastAll(transform.position + (direction * (distToFace)),0.1f,direction, 0.0f,enemyLayerMask );
                foreach(var hit in hits)
                {
                    Player enemy = hit.collider.GetComponentInParent<Player>();
                    if (enemy && enemy != this)
                    {
                        Debug.Log("Slowing down");
                        maxSpeed = oldSpeed * 0.1f;
                        colliding = true;
                    }
                }
                if (!colliding)
                    maxSpeed = oldSpeed;

                    
                if (newVelocity.magnitude > maxSpeed)
                    newVelocity = newVelocity.normalized * maxSpeed;

                if (isBlocking)
                    newVelocity *= 0.5f;

                rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.y);
            }
            else
            {
                //Stop movement
                float multiplier = isGrounded ? 0.2f : 1f;
                rb.velocity = new Vector3(rb.velocity.x *multiplier,rb.velocity.y, rb.velocity.z*multiplier);
            }

        }
        else if (isPushing && !isStunned)
        {
            Vector3 euler = new Vector3(0, Mathf.Atan2(state.ThumbSticks.Left.X, state.ThumbSticks.Left.Y) * 180 / Mathf.PI, 0);
            if (euler.magnitude > 0.2f)
                transform.rotation = Quaternion.Lerp(Quaternion.Euler(transform.eulerAngles), Quaternion.Euler(euler), Time.deltaTime);
            
            Vector3 vel = transform.forward;

            if (!IsPushingEnemy())
                vel *= maxSpeed*1.25f;
            else
                vel = lastEnemyContact.rb.velocity;

            rb.velocity = new Vector3(vel.x, rb.velocity.y,vel.z);
        }
    }
    void HandleRotating(GamePadState state)
    {
        if (!isPushing)
        {
            rotAxisH = state.ThumbSticks.Right.X;
            rotAxisV = state.ThumbSticks.Right.Y;

            if (rotAxisH > rotationDeadzone || rotAxisH < -rotationDeadzone || rotAxisV > rotationDeadzone || rotAxisV < -rotationDeadzone)
            {
                transform.eulerAngles = new Vector3(0, Mathf.Atan2(rotAxisH, rotAxisV) * 180 / Mathf.PI, 0);
            }
        }

    }
    void HandleActions(GamePadState state, GamePadState prevState)
    {
        // Detect if a button was pressed this frame
        if (!isGrabbed && !isPushed)
        {
            if (!isGrabbing)
            {

                if (isGrounded && ((prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed) ||
                (state.Buttons.A == ButtonState.Pressed && prevState.Buttons.A == ButtonState.Released)))
                {
                    Jump();
                }

                //Charging weapon if hasWeapon and pushing if !hasWeapon
                if (!isHitting && !isPushing && state.Buttons.RightShoulder == ButtonState.Pressed && state.Buttons.LeftShoulder == ButtonState.Pressed)
                {
                    //Every frame
                    hitInput = false;
                    Block();
                }
                if (isBlocking && state.Buttons.RightShoulder == ButtonState.Released && state.Buttons.LeftShoulder == ButtonState.Released)
                {
                    //Once
                    EndBlock();
                }

                if (!isHitting && !weaponCharging && !weaponCharged && isGrounded && state.Triggers.Left > 0.2f && state.Triggers.Right > 0.2f && (prevState.Triggers.Left < 0.2f || prevState.Triggers.Right < 0.2f))
                {
                    //Every frame
                    Push();
                }
                if (isPushing && state.Triggers.Left < 0.2f && state.Triggers.Right < 0.2f)
                {
                    EndPush();
                }

                if (!isPushing && !isBlocking && state.Buttons.RightShoulder == ButtonState.Pressed && state.Buttons.LeftShoulder == ButtonState.Released)
                {
                    leftHand = false;
                    hitInput = true;
                    if (hitInputTimer > 0.033f)
                        Hit(); //Hit with HAND
                }

                if (!isPushing && !isBlocking && state.Buttons.LeftShoulder == ButtonState.Pressed && state.Buttons.RightShoulder == ButtonState.Released)
                {
                    leftHand = true;
                    hitInput = true;
                    if (hitInputTimer > 0.033f)
                        Hit(); //Hit with HAND
                }

                if (    (prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed) ||
                        (prevState.Buttons.B == ButtonState.Released && state.Buttons.B == ButtonState.Pressed))
                {
                    Grab();
                }
            }
            else if (isGrabbing)
            {
                if (state.Triggers.Left > 0.2f && state.Triggers.Right > 0.2f)
                {
                    if (currentWeapon && !weaponCharging && !weaponCharged && !isHitting) //Has weapon
                        Charge();
                    else if (!currentWeapon) //Grabbing player
                        EndGrab();
                }
                if(currentWeapon && weaponCharged && (state.Triggers.Left < 0.2f && state.Triggers.Right < 0.2f))
                {
                    Hit(); //Hits with weapon
                }
                
            }
        }
        else if (isPushed)
        {
            //Charging weapon if hasWeapon and pushing if !hasWeapon
            if (state.Buttons.RightShoulder == ButtonState.Pressed && state.Buttons.LeftShoulder == ButtonState.Pressed && (prevState.Buttons.RightShoulder == ButtonState.Released || prevState.Buttons.LeftShoulder == ButtonState.Released))
            {
                //Every frame
                Block();
            }
        }




    }
    void HandleMenuInputs(GamePadState state, GamePadState prevState)
    {
        if (state.Buttons.Start == ButtonState.Pressed && prevState.Buttons.Start == ButtonState.Released)
        {

        }
        if (state.Buttons.Back == ButtonState.Pressed && prevState.Buttons.Back == ButtonState.Released)
        {
            //Maybe indication of which player is yours??
        }
    }


    void Jump()
    {
        anim.SetTrigger("jump");
        anim.SetBool("isGrounded", false);
        isGrounded = false;
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    void Charge()
    {
        if (currentWeapon && !weaponCharging)
        {
            anim.SetBool("isCharging", true);
            
            weaponCharging = true;
            Invoke("SetCharged", currentWeapon.chargeTime);
        }
    }
    void SetCharged() //Invoked from Charge()
    {
        anim.SetBool("isCharged", true);
        anim.SetBool("isCharging", false);
        weaponCharged = true;
        weaponCharging = false;
    }

    void Hit()
    {
        hitInput = false;
        if (currentWeapon && weaponCharged && !isHitting)
        {
                //Weapon hits
                isHitting = true;
                weaponCharged = false;
                anim.SetBool("isHitting", true);
                anim.SetBool("isCharged", false);
                currentWeapon.hitCollider.enabled = true;
                Invoke("EndHit", currentWeapon.hitTime);
                Invoke("EndGrab", currentWeapon.hitTime);
        }
        else if (!currentWeapon && !isHitting)
        {
            isHitting = true;
            anim.SetBool("isHitting", true);
            //if lefthand ->    hit with left hand
            //else ->           hit with right hand
            Invoke("CheckHit", hitTime/2);
            Invoke("EndHit", hitTime);
        }

        weaponCharged = false;
    }
    void CheckHit()
    {
        Debug.Log("Check hit");
        RaycastHit[] hits = Physics.SphereCastAll(transform.position + (transform.forward *(distToFace+0.5f)), 0.5f, transform.forward, 0.2f, enemyLayerMask);
        List<Player> hitPlayers = new List<Player>();
        foreach(var hit in hits)
        {
            Player enemy = hit.collider.GetComponentInParent<Player>();
            if (enemy && !hitPlayers.Contains(enemy) && enemy != this)
            {
                hitPlayers.Add(enemy);
                enemy.GetHit(damage);
                Debug.Log("Dealt damage: " + damage + " to enemy: " + enemy);
            }
        }
    }
    void EndHit() //Invoked from Hit()
    {
        anim.SetBool("isHitting", false);
        if (currentWeapon)
            currentWeapon.hitCollider.enabled = false;
        isHitting = false;
    }

    void Push()
    {
        anim.SetBool("isPushing", true);
        isPushing = true;
    }
    void EndPush()
    {
        anim.SetBool("isPushing", false);
        isPushing = false;
        if (lastEnemyContact)
            lastEnemyContact.GetPushed(false, this);
    }

    void Block()
    {
        anim.SetBool("isBlocking", true);
        isBlocking = true;
    }
    void EndBlock()
    {
        anim.SetBool("isBlocking", false);
        isBlocking = false;
    }
    void Grab()
    {
        //Some kind of trigger/sphere check about what is in front of player

        //If weapon pickup
            //Move it to hand bone
            //Assign as currentWeapon
            //Set animation speed (variable maybe in Weapon)
        
        //if another player
            //Give other player "isGrabbed" to make its actions disabled
        currentWeapon.player = this;
        anim.SetBool("isGrabbing", true);
        isGrabbing = true;

    }
    void EndGrab()
    {
        //Detach enemy/weapon from bone
        //Throw enemy/weapon with physics force
        anim.SetBool("hasWeapon", false);
        currentWeapon = null;
        anim.SetBool("isGrabbing", false);
        isGrabbing = false;
    }

    public void FlushAllActionData()
    {
        weaponCharging = false;
        weaponCharged = false;
        isGrabbing = false;
        isPushing = false;
        isBlocking = false;
        isHitting = false;
        leftHand = false;
        currentWeapon = null;
    }



    public void AddBuff(bool stunned, bool grabbed, float time)
    {
        if (isStunned || isGrabbed)
            return;
        isStunned = stunned;
        isGrabbed = grabbed;
        stunTime = time;
        anim.SetBool("isStunned", isStunned);
        anim.SetBool("isGrabbed", isGrabbed);
        StartCoroutine(HandleBuff(stunned));

    }

    IEnumerator HandleBuff(bool stunned)
    {
        if (stunned)
        {
            //Stun particlesystem play
            //Stun sound play
            yield return new WaitForSecondsRealtime(stunTime);
            anim.SetBool("isStunned", false);
            isStunned = false;
            //Stun particlesystem stop
            //Stun sound stop
        }
    }

    public void GetHit(int dmgValue)
    {

        if (health.isAlive())
        {
            FMODUnity.RuntimeManager.PlayOneShot(damageSound, transform.position);
        }

        //Flush some shit
        weaponCharged = weaponCharging = false;
        EndGrab();
        EndPush();
        EndHit();

        anim.SetTrigger("takeDamage");
        health.TakeDamage(dmgValue);

        if (takeDamageParticles)
        {
            Quaternion rot = transform.rotation;
            rot.y = Random.Range(0, 360);
            GameObject temp = Instantiate(takeDamageParticles, transform.position, rot);
            Destroy(temp, 2f);
        }

        if (!health.isAlive())
        {
            AddBuff(true, false, 1f);
            FMODUnity.RuntimeManager.PlayOneShot(getKnockedOutBitchSound, transform.position);
        }

    }


    void DIE()
    {
        //Play dead animation
        if (!destroying)
        {
            // animControl.PlayDeathAnimation();
            if (deathSound != "")
                FMODUnity.RuntimeManager.PlayOneShot(deathSound, transform.position);
            destroying = true;
        }

    }

    public void TakeOut()
    {
        //DIE()???
        gameObject.SetActive(false);
    }
}
