using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerDebug : MonoBehaviour {


	[Tooltip("Routes all gamepad inputs into this player")]
	public bool overrideInputs;
	Player player;



    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;

	void Start()
	{
		if (!player)
			player = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {



		if (overrideInputs)
		{
			if (!playerIndexSet || !prevState.IsConnected)
			{
				for (int i = 0; i < 4; ++i)
				{
					PlayerIndex testPlayerIndex = (PlayerIndex)i;
					GamePadState testState = GamePad.GetState(testPlayerIndex);
					if (testState.IsConnected)
					{
						Debug.Log(string.Format("GamePad found {0}", testPlayerIndex));
						playerIndex = testPlayerIndex;
						playerIndexSet = true;
					}
				}
			}

			prevState = state;
			state = GamePad.GetState(playerIndex);

			if (state.IsConnected)
				player.HandleInput(state, prevState);
			
		}
		
	}
}
