using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class InputHandler : MonoBehaviour {

	List<Player> players = new List<Player>();
	GamepadStateHandler gamepad;
	StateHandler stateHandler; 

	void Awake()
	{
		gamepad = GetComponent<GamepadStateHandler>();
		stateHandler = GetComponent<StateHandler>();
	}
	public void HandleInput(PlayerGamepadData[] gamepadData, int i)
	{
		if (stateHandler.gamestate == GameState.menu)
		{
			//Handle player (gamepad) activation - check if Y-button was pressed in this gamepad in this frame
			if (gamepadData[i].prevState.Buttons.Y == ButtonState.Released && gamepadData[i].state.Buttons.Y == ButtonState.Pressed)
			{
				//Invert active bool
				gamepadData[i].active = !gamepadData[i].active;

				if (gamepadData[i].active)
				{
					Playerdata newPlayerdata = new Playerdata();

					//Index should be first null
					for(int j = 0; j < gamepad.playerDataArray.Length; j++)
					{
						if (gamepad.playerDataArray[j] == null)
						{
							newPlayerdata.playerIndex = j;
							break;
						}
					}

					gamepadData[i].playerIndex = newPlayerdata.playerIndex;
					newPlayerdata.gamepadIndex = gamepadData[i].gamepadIndex;

					bool hasPlayer = false; //temp
					for(int j = 0; j < gamepad.playerDataArray.Length; j++)
					{
						//if player's assigned gamepad is same as current gamepad which pressed join
						if (gamepad.playerDataArray[j] != null && gamepad.playerDataArray[j].gamepadIndex == gamepad.playerGamepadData[i].gamepadIndex )
							hasPlayer = true; //Won't add
					}
					if (!hasPlayer)
					{
						for(int j = 0; j < gamepad.playerDataArray.Length; j++)
						{
							if (gamepad.playerDataArray[j] == null)
							{
								gamepad.playerDataArray[j] = newPlayerdata;
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
					for (int j = 0; j < gamepad.playerDataArray.Length; j++)
					{
						if (gamepad.playerDataArray[j] != null && gamepad.playerDataArray[j].gamepadIndex == gamepadData[i].gamepadIndex)
						{
							stateHandler.menuControl.RemovePlayer(gamepad.playerDataArray[j].playerIndex);
							Debug.Log("deactivation of gamepad: " + gamepad.playerDataArray[j].gamepadIndex + ", playerindex: " + gamepad.playerDataArray[j].playerIndex);
							gamepad.playerDataArray[j] = null;
						}
					}
				}
			}

			if (gamepadData[i].active && gamepadData[i].prevState.Buttons.A == ButtonState.Released && gamepadData[i].state.Buttons.A == ButtonState.Pressed)
			{
				stateHandler.menuControl.ToggleReady(gamepadData[i].playerIndex);
			}
			if (gamepadData[i].active && gamepadData[i].prevState.ThumbSticks.Left.X < 0.1f && gamepadData[i].state.ThumbSticks.Left.X > 0.1f)
			{
				if (stateHandler.menuControl.avatars)
					stateHandler.menuControl.avatars.ChangeAvatar(gamepadData[i].playerIndex, 1);
			}
			if (gamepadData[i].active && gamepadData[i].prevState.ThumbSticks.Left.X > -0.1f && gamepadData[i].state.ThumbSticks.Left.X < -0.1f)
			{
				if (stateHandler.menuControl.avatars)
					stateHandler.menuControl.avatars.ChangeAvatar(gamepadData[i].playerIndex, -1);
			}
		}
		if (stateHandler.gamestate == GameState.game)
		{
			
		}
	}
	
}
