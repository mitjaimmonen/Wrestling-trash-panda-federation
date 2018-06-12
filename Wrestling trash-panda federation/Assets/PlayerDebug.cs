using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerDebug : MonoBehaviour {


	[Tooltip("Routes all gamepad inputs into this player")]
	public bool overrideInputs;
	[SerializeField] bool variableOverride;
	[SerializeField] bool hasWeapon;
	[SerializeField] bool weaponCharged;
	[SerializeField] bool isPushing;
	[SerializeField] bool isBlocking;
	[SerializeField] bool isHitting;
	[SerializeField] bool leftHand;
	[SerializeField] Weapon currentWeapon;
	gamepads[] gamepadsArray = new gamepads[4];
	Player player;

	public struct gamepads
	{
		public GamePadState state;
		public GamePadState prevState;
		public int index;
	}



	void Start () {

		for (int i = 0; i < gamepadsArray.Length; i++)
		{
			PlayerIndex testPlayerIndex = (PlayerIndex)i;
			GamePadState testState = GamePad.GetState(testPlayerIndex);

			// Debug.Log(string.Format("GamePad {0}", testPlayerIndex));

			gamepadsArray[i].state = testState;
			gamepadsArray[i].index = i;
		}
		player = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		if (variableOverride)
			OverrideVariables();

		if (overrideInputs)
		{
			for(int i =0; i < 4; i++)
			{
				gamepadsArray[i].prevState = gamepadsArray[i].state;
				gamepadsArray[i].state = GamePad.GetState((PlayerIndex)gamepadsArray[i].index);
				if (gamepadsArray[i].state.IsConnected)
					player.HandleInput(gamepadsArray[i].state, gamepadsArray[i].prevState);
			}
		}
		
	}

	void OverrideVariables()
	{
		player.hasWeapon = hasWeapon;
		player.weaponCharged = weaponCharged;
		player.isPushing = isPushing;
		player.isBlocking = isBlocking;
		player.isHitting = isHitting;
		player.leftHand = leftHand;
	}
}
