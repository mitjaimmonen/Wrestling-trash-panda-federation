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
    public GameObject takeDamageParticles;
    public float movementSpeed;
    [Range(0.01f, 0.99f), Tooltip("Deadzone for joystick movement (Left stick).")]
    public float movementDeadzone = 0.05f;
    [Range(0.01f, 0.99f), Tooltip("Deadzone for joystick rotation (Right stick).")]
    public float rotationDeadzone = 0.05f;


    [FMODUnity.EventRef] public string damageSound;
    [FMODUnity.EventRef] public string deathSound;
    [FMODUnity.EventRef] public string getKnockedOutBitchSound;

    protected Animator anim;

    Health health;
    bool stunBuff = false;
    bool grabbedBuff = false;
    float buffTime;


    #region Actions data
    public float chargeTime = 0.1f;
    public float hitTime = 0.1f;
    [HideInInspector] public bool hasWeapon = false;
    [HideInInspector] public bool weaponCharging = false;
    [HideInInspector] public bool weaponCharged = false;
    [HideInInspector] public bool isGrabbing = false;
    [HideInInspector] public bool isPushing = false;
    [HideInInspector] public bool isBlocking = false;
    [HideInInspector] public bool isHitting = false;
    [HideInInspector] public bool leftHand = false;
    [HideInInspector] public Weapon currentWeapon;



    #endregion



    float moveAxisV; //Stores input values 0-1
    float moveAxisH;
    float rotAxisV;
    float rotAxisH;

    bool destroying;

    //public Player(Playerdata _playerData)
    //{
    //    playerData = _playerData;

    //    playerNumber = playerData.playerIndex;
    //    meshNumber = playerData.meshNumber;

    //}

    private void Awake()
    {
        health = new Health(maxHealth);

        anim = GetComponentInChildren<Animator>();     

    }

    private void Start()
    {
       
    }

    void Update()
    {

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
            if (!stunBuff)
            {
                if (grabbedBuff)
                {
                    HandleMove(state);
                    HandleActions(state, prevState);
                }
                else
                {
                    HandleMove(state);
                    HandleRotating(state);
                    HandleActions(state, prevState);

                }

            }

            HandleMenuInputs(state, prevState);

        }
    }

    void HandleMove(GamePadState state)
    {
        moveAxisH = state.ThumbSticks.Left.X;
        moveAxisV = state.ThumbSticks.Left.Y;

        // animControl.SetVerticalMagnitude(moveAxisH);

        if (moveAxisH > movementDeadzone || moveAxisH < -movementDeadzone)
        {
            //Movement
            float magnitude = moveAxisH < 0 ? moveAxisH + movementDeadzone : moveAxisH - movementDeadzone;
            transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.right * magnitude, Time.deltaTime * movementSpeed);
            // animControl.SetVerticalMagnitude(moveAxisH);   
        }
        if (moveAxisV > movementDeadzone || moveAxisV < -movementDeadzone)
        {
            float magnitude = moveAxisV < 0 ? moveAxisV + movementDeadzone : moveAxisV - movementDeadzone;
            var cameraforw = Vector3.forward;

            cameraforw.y = 0;
            cameraforw.Normalize();
            transform.position = Vector3.Lerp(transform.position, transform.position + cameraforw * magnitude, Time.deltaTime * movementSpeed);
            // animControl.SetVerticalMagnitude(moveAxisV);
        }
    }
    void HandleRotating(GamePadState state)
    {
        rotAxisH = state.ThumbSticks.Right.X;
        rotAxisV = state.ThumbSticks.Right.Y;

        if (rotAxisH > rotationDeadzone || rotAxisH < -rotationDeadzone || rotAxisV > rotationDeadzone || rotAxisV < -rotationDeadzone)
        {
            transform.eulerAngles = new Vector3(0, Mathf.Atan2(rotAxisH, rotAxisV) * 180 / Mathf.PI, 0);
        }
    }
    void HandleActions(GamePadState state, GamePadState prevState)
    {
        // Detect if a button was pressed this frame
        if (!isGrabbing)
        {
            //Charging weapon if hasWeapon and pushing if !hasWeapon
            if (!isHitting && !isPushing && (state.Buttons.RightShoulder == ButtonState.Pressed && state.Buttons.LeftShoulder == ButtonState.Pressed))
            {
                //Every frame
                Block();
            }

            if (!isHitting && !weaponCharging && !weaponCharged && state.Triggers.Left > 0.2f && state.Triggers.Right > 0.2f)
            {
                //Every frame
                Push();
            }
            if (isPushing && state.Triggers.Left < 0.2f && state.Triggers.Right < 0.2f)
            {
                EndPush();
            }

            if (!isPushing && prevState.Buttons.RightShoulder == ButtonState.Released && state.Buttons.RightShoulder == ButtonState.Pressed && state.Buttons.LeftShoulder == ButtonState.Released)
            {
                leftHand = true;
                Hit(); //Hit with HAND
            }

            if (!isPushing && prevState.Buttons.LeftShoulder == ButtonState.Released && state.Buttons.LeftShoulder == ButtonState.Pressed && state.Buttons.RightShoulder == ButtonState.Released)
            {
                leftHand = false;
                Hit(); //Hit with HAND
            }

            if (!isGrabbing &&
                    (prevState.Buttons.X == ButtonState.Released && state.Buttons.X == ButtonState.Pressed) ||
                    (prevState.Buttons.Y == ButtonState.Released && state.Buttons.Y == ButtonState.Pressed) ||
                    (prevState.Buttons.A == ButtonState.Released && state.Buttons.A == ButtonState.Pressed) ||
                    (prevState.Buttons.B == ButtonState.Released && state.Buttons.B == ButtonState.Pressed))
            {
                Grab();
            }
        }
        else //isGrabbing == true
        {
            if ((state.Buttons.RightShoulder == ButtonState.Pressed && state.Buttons.LeftShoulder == ButtonState.Pressed)
                || (state.Triggers.Left > 0.2f && state.Triggers.Right > 0.2f))
            {
                if (hasWeapon)
                {
                    if (currentWeapon && weaponCharged)
                        Hit(); //Hits with weapon
                }
                else //Grabbing player
                    EndGrab();
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



    void Charge()
    {
        if (hasWeapon && currentWeapon && !weaponCharging)
        {
            weaponCharging = true;
            Invoke("SetCharged", chargeTime);

        }
    }
    void SetCharged() //Invoked from Charge()
    {
        weaponCharged = true;
    }

    void Hit()
    {
        if (hasWeapon)
        {
            if (currentWeapon && weaponCharged)
            {
                //Weapon hits
                isHitting = true;
                Invoke("EndHit", currentWeapon.hitTime);
                Invoke("EndGrab", currentWeapon.hitTime);
            }
        }
        else
        {
            isHitting = true;
            //if lefthand ->    hit with left hand
            //else ->           hit with right hand
            Invoke("EndHit", hitTime);
        }

        weaponCharged = false;
    }
    void EndHit() //Invoked from Hit()
    {
        isHitting = false;
    }

    void Push()
    {

    }
    void EndPush()
    {

    }

    void Block()
    {
        isBlocking = true;
    }
    void EndBlock()
    {

    }
    void Grab()
    {
        //Some kind of trigger/sphere check about what is in front of player

        //If weapon pickup
        //Move it to hand bone
        //Assign as currentWeapon
        //Set animation speed (variable maybe in Weapon)
        //hasWeapon = true;

        //if another player
        //Give other player "grabbedBuff" to make its actions disabled
        isGrabbing = true;

    }
    void EndGrab()
    {
        //Detach enemy from bone
        //Throw enemy with physics force
        isGrabbing = false;
    }

    public void AddBuff(bool stunned, bool grabbed, float time)
    {
        if (stunBuff)
            return;
        stunBuff = stunned;
        grabbedBuff = grabbed;
        buffTime = time;

        StartCoroutine(HandleBuff(stunned));

    }

    IEnumerator HandleBuff(bool stunned)
    {
        if (stunned)
        {
            yield return new WaitForSecondsRealtime(buffTime);
            stunBuff = false;
        }
    }

    public void GetHit(int dmgValue)
    {

        if (health.isAlive())
        {
            FMODUnity.RuntimeManager.PlayOneShot(damageSound, transform.position);
        }

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
}
