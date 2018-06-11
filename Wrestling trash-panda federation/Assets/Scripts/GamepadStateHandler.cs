using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;


public struct PlayerGamepadData
{
	public GamePadState state;
	public GamePadState prevState;
	public int gamepadIndex;
	public int playerIndex;
	public bool active;

}

public class GamepadStateHandler : MonoBehaviour {

	[HideInInspector]public Playerdata[] playerDataArray = new Playerdata[4];
	[HideInInspector]public PlayerGamepadData[] playerGamepadData = new PlayerGamepadData[4];
	[HideInInspector]public StateHandler stateHandler;
	[HideInInspector]public Avatars avatars;


	public GameObject[] testObj = new GameObject[4];
	InputHandler inputHandler;


	float timer = 0;

    // Use this for initialization
    void Awake()
    {
		stateHandler = GetComponent<StateHandler>();
		inputHandler = GetComponent<InputHandler>();
		FindGamepads();

		for(int i = 0; i < playerDataArray.Length;i++)
		{
			playerDataArray[i] = null;
		}
    }

	void FindGamepads()
	{
		for (int i = 0; i < 4; ++i)
		{
			PlayerIndex testPlayerIndex = (PlayerIndex)i;
			GamePadState testState = GamePad.GetState(testPlayerIndex);

			// Debug.Log(string.Format("GamePad {0}", testPlayerIndex));

			PlayerGamepadData gamepad = new PlayerGamepadData();
			gamepad.state = GamePad.GetState(testPlayerIndex);
			gamepad.prevState = gamepad.state;
			gamepad.gamepadIndex = i;
			gamepad.playerIndex = -1;
			gamepad.active = false;

			playerGamepadData[i] = gamepad;
			
		}
	}

    // Update is called once per frame
    void Update()
    {

		//Go through all gamepads
		for (int i = 0; i < playerGamepadData.Length; i++)
		{
			//Save old state for button press checking
       		playerGamepadData[i].prevState = playerGamepadData[i].state;
			//Get current state of gamepad
        	playerGamepadData[i].state = GamePad.GetState((PlayerIndex)playerGamepadData[i].gamepadIndex);

			if (!playerGamepadData[i].state.IsConnected)
				continue;
				
			//Handle player (gamepad) activation - check if A-button was pressed in this gamepad in this frame
			if (playerGamepadData[i].prevState.Buttons.Y == ButtonState.Released && playerGamepadData[i].state.Buttons.Y == ButtonState.Pressed)
			{
				//Invert active bool
				playerGamepadData[i].active = !playerGamepadData[i].active;

				if (playerGamepadData[i].active)
				{
					Playerdata newPlayerdata = new Playerdata();

					//Index should be first null
					for(int j = 0; j < playerDataArray.Length; j++)
					{
						if (playerDataArray[j] == null)
						{
							newPlayerdata.playerIndex = j;
							break;
						}
					}

					playerGamepadData[i].playerIndex = newPlayerdata.playerIndex;
					newPlayerdata.gamepadIndex = playerGamepadData[i].gamepadIndex;

					bool hasPlayer = false; //temp
					for(int j = 0; j < playerDataArray.Length; j++)
					{
						//if player's assigned gamepad is same as current gamepad which pressed join
						if (playerDataArray[j] != null && playerDataArray[j].gamepadIndex == playerGamepadData[i].gamepadIndex )
							hasPlayer = true; //Won't add
					}
					if (!hasPlayer)
					{
						for(int j = 0; j < playerDataArray.Length; j++)
						{
							if (playerDataArray[j] == null)
							{
								playerDataArray[j] = newPlayerdata;
								Debug.Log("Added to array");
								break;
							}
						}
						Debug.Log("activation of gamepad:  " + newPlayerdata.gamepadIndex + ", playerindex: " + newPlayerdata.playerIndex);
						//Updates menu accordingly
							
						stateHandler.menuControl.AddPlayer(newPlayerdata.playerIndex);
					}
				}
				else
				{
					for (int j = 0; j < playerDataArray.Length; j++)
					{
						if (playerDataArray[j] != null && playerDataArray[j].gamepadIndex == playerGamepadData[i].gamepadIndex)
						{
							stateHandler.menuControl.RemovePlayer(playerDataArray[j].playerIndex);
							Debug.Log("deactivation of gamepad: " + playerDataArray[j].gamepadIndex + ", playerindex: " + playerDataArray[j].playerIndex);
							playerDataArray[j] = null;
						}
					}
				}


			}
			if (playerGamepadData[i].active && playerGamepadData[i].prevState.Buttons.A == ButtonState.Released && playerGamepadData[i].state.Buttons.A == ButtonState.Pressed)
			{
				stateHandler.menuControl.ToggleReady(playerGamepadData[i].playerIndex);
			}
			if (playerGamepadData[i].active && playerGamepadData[i].prevState.ThumbSticks.Left.X < 0.1f && playerGamepadData[i].state.ThumbSticks.Left.X > 0.1f)
			{
				if (avatars)
					avatars.ChangeAvatar(playerGamepadData[i].playerIndex, 1);
			}
			if (playerGamepadData[i].active && playerGamepadData[i].prevState.ThumbSticks.Left.X > -0.1f && playerGamepadData[i].state.ThumbSticks.Left.X < -0.1f)
			{
				if (avatars)
					avatars.ChangeAvatar(playerGamepadData[i].playerIndex, -1);
			}

			//Handles rest of the inputs.
			if (inputHandler)
				inputHandler.HandleInput(playerGamepadData);

		}
    }

    void OnGUI()
    {
		string text = "";
		for (int i = 0; i < playerGamepadData.Length; i++)
		{
			if (!playerGamepadData[i].active || !playerGamepadData[i].state.IsConnected)
			{
				continue;
			}

			text += "Use left stick to turn the cube, hold A to change color\n";
			text += string.Format("IsConnected {0} Packet #{1} Active {2}\n", playerGamepadData[i].state.IsConnected, playerGamepadData[i].state.PacketNumber, playerGamepadData[i].active);
			text += string.Format("\tTriggers {0} {1} {2}\n", playerGamepadData[i].state.Triggers.Left, playerGamepadData[i].state.Triggers.Right, (PlayerIndex)playerGamepadData[i].gamepadIndex);
			// text += string.Format("\tD-Pad {0} {1} {2} {3}\n", gamepads[i].state.DPad.Up, gamepads[i].state.DPad.Right, gamepads[i].state.DPad.Down, gamepads[i].state.DPad.Left);
			// text += string.Format("\tButtons Start {0} Back {1} Guide {2}\n", gamepads[i].state.Buttons.Start, gamepads[i].state.Buttons.Back, gamepads[i].state.Buttons.Guide);
			// text += string.Format("\tButtons LeftStick {0} RightStick {1} LeftShoulder {2} RightShoulder {3}\n", gamepads[i].state.Buttons.LeftStick, gamepads[i].state.Buttons.RightStick, gamepads[i].state.Buttons.LeftShoulder, gamepads[i].state.Buttons.RightShoulder);
			text += string.Format("\tButtons A {0} B {1} X {2} Y {3}\n", playerGamepadData[i].state.Buttons.A, playerGamepadData[i].state.Buttons.B, playerGamepadData[i].state.Buttons.X, playerGamepadData[i].state.Buttons.Y);
			text += string.Format("\tSticks Left {0} {1} Right {2} {3}\n", playerGamepadData[i].state.ThumbSticks.Left.X, playerGamepadData[i].state.ThumbSticks.Left.Y, playerGamepadData[i].state.ThumbSticks.Right.X, playerGamepadData[i].state.ThumbSticks.Right.Y);
		}
		GUI.Label(new Rect(0, 0, Screen.width, Screen.height), text);

    }
}
