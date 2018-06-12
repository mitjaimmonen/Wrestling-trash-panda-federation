using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;


public struct PlayerGamepadData
{

	//Store gamepad data of each player

	public GamePadState state; //Button states etc
	public GamePadState prevState; //Button states etc from last frame
	public int gamepadIndex; //Each gamepad gets assigned a number
	public int playerIndex; //Each player gets assign
	public bool active; //When joined game, gamepad becomes active

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
			//Get current state of gamepad (all of its buttonstates)
        	playerGamepadData[i].state = GamePad.GetState((PlayerIndex)playerGamepadData[i].gamepadIndex);

			if (!playerGamepadData[i].state.IsConnected)
				continue;
				
			if (inputHandler)
				inputHandler.HandleInput(playerGamepadData, i);

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
