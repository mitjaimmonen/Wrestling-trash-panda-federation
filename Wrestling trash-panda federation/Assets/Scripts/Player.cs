using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class Player : MonoBehaviour {

	public int playerNumber;
	public int meshNumber;

	[Tooltip("List of player skins (prefabs). Player chooses which one gets instantiated at main menu joining.")]
	public GameObject[] modelPrefabs = new GameObject[4];


    public float movementSpeed;
    [Range(0.01f, 0.99f), Tooltip("Deadzone for joystick movement (Left stick).")]
    public float movementDeadzone = 0.05f;
    [Range(0.01f, 0.99f), Tooltip("Deadzone for joystick rotation (Right stick).")]
    public float rotationDeadzone = 0.05f;
    public CapsuleCollider playerCollider;
    public GameObject takeDamageParticles;
    public int maxHealth;
    [FMODUnity.EventRef, Tooltip("Ouch-sound")] public string damageSound;
    [FMODUnity.EventRef] public string deathSound;

    // protected AnimControl animControl;

     Health health;
     bool stunBuff = false;
     float buffTime;


	 bool hasWeapon;
	 bool weaponCharged;
	 bool isPushing;
	 bool isBlocking;
	 bool isHitting;
	 bool leftHand;
	 Weapon currentWeapon;




    float moveAxisV; //Stores input values 0-1
    float moveAxisH;
    float rotAxisV;
    float rotAxisH;

     bool destroying;


    private void Awake()
    {
        health = new Health(maxHealth);

        // animControl = new PlayerAnimControl(GetComponentInChildren<Animator>());
    }


    public void HandleInput(GamePadState state, GamePadState prevState)
    {
        if (health.isAlive())
        {
			if (!stunBuff)
			{
            	HandleMove(state);
            	HandleRotating(state);
            	HandleAttacks(state, prevState);
			}

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
	void HandleAttacks(GamePadState state, GamePadState prevState)
    {
        // Detect if a button was pressed this frame
		if (state.Buttons.RightShoulder == ButtonState.Pressed && state.Buttons.LeftShoulder == ButtonState.Pressed)
		{
			if (hasWeapon && currentWeapon)
			{
				//Weapon Charge
				weaponCharged = true;
			}
			else
			{
				isPushing = true;
				//Push
			}
		}
		if (state.Triggers.Left > 0.2f && state.Triggers.Right > 0.2f)
		{
			if (hasWeapon && currentWeapon)
			{
				weaponCharged = true;
				//Weapon charge
			}
			else
			{
				isBlocking = true;
				//Block when pressing both triggers
			}
		}

		if (weaponCharged && hasWeapon && currentWeapon && state.Triggers.Left < 0.2f && state.Triggers.Right < 0.2f)
		{
			//Hit with weapon
			weaponCharged = false;
			isHitting = true;
		}


        if (prevState.Buttons.RightShoulder == ButtonState.Released && state.Buttons.RightShoulder == ButtonState.Pressed && state.Buttons.LeftShoulder == ButtonState.Released)
        {
			leftHand = true;
			isHitting = true;
        }

        if (prevState.Buttons.LeftShoulder == ButtonState.Released && state.Buttons.LeftShoulder == ButtonState.Pressed && state.Buttons.RightShoulder == ButtonState.Released)
        {
			leftHand = false;
			isHitting = true;
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


	public void Attack()
    {
        //DO ATTACK????!?!
    }



    public virtual void Move()
    {

    }

    public void AddBuff(bool addIceBuff, bool addStunBuff, float time)
    {
        if (stunBuff)
            return;
        stunBuff = addStunBuff;
        buffTime = time;

        StartCoroutine(HandleBuff(addStunBuff));

    }

    IEnumerator HandleBuff(bool stun)
    {
        yield return new WaitForSecondsRealtime(buffTime);

        if (stun)
            stunBuff = false;
    }

    public void GetHit(float dmgValue)
    {

        if (health.isAlive())
        {
            // animControl.PlayHurtAnimation();
            FMODUnity.RuntimeManager.PlayOneShot(damageSound, transform.position);
        }

        // health.TakeDamage(dmgValue);

        if (takeDamageParticles)
        {
            Quaternion rot = transform.rotation;
            rot.y = Random.Range(0, 360);
            GameObject temp = Instantiate(takeDamageParticles, transform.position, rot);
            Destroy(temp, 2f);
        }




        if (!health.isAlive())
            DIE();

    }
}
