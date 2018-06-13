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
	public PlayerGamepadData HandleInput(PlayerGamepadData gamepadData)
	{
		if (stateHandler.gamestate == GameState.menu)
		{
			HandleMenuInputs(gamepadData);
		}
		if (stateHandler.gamestate == GameState.game)
		{
			players[gamepadData.playerIndex].HandleInput(gamepadData.state, gamepadData.prevState);
		}

		return gamepadData;
	}

	public PlayerGamepadData HandleFixedInput(PlayerGamepadData gamepadData)
	{
		if (stateHandler.gamestate == GameState.game)
		{
			players[gamepadData.playerIndex].HandleFixedInput(gamepadData.state, gamepadData.prevState);
		}
		return gamepadData;
	}
	

	void HandleMenuInputs(PlayerGamepadData gamepadData)
	{
		//Handle player (gamepad) activation - check if Y-button was pressed in this gamepad in this frame
		if (gamepadData.prevState.Buttons.Y == ButtonState.Released && gamepadData.state.Buttons.Y == ButtonState.Pressed)
		{
			//Invert active bool
			gamepadData.active = !gamepadData.active;
			Debug.Log("Gamepad active: "+gamepadData.active);
			if (gamepadData.active)
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

				gamepadData.playerIndex = newPlayerdata.playerIndex;
				newPlayerdata.gamepadIndex = gamepadData.gamepadIndex;

				bool hasPlayer = false; //temp
				for(int j = 0; j < gamepad.playerDataArray.Length; j++)
				{
					//if player's assigned gamepad is same as current gamepad which pressed join
					if (gamepad.playerDataArray[j] != null && gamepad.playerDataArray[j].gamepadIndex == gamepadData.gamepadIndex )
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
					if (gamepad.playerDataArray[j] != null && gamepad.playerDataArray[j].gamepadIndex == gamepadData.gamepadIndex)
					{
						stateHandler.menuControl.RemovePlayer(gamepad.playerDataArray[j].playerIndex);
						Debug.Log("deactivation of gamepad: " + gamepad.playerDataArray[j].gamepadIndex + ", playerindex: " + gamepad.playerDataArray[j].playerIndex);
						gamepad.playerDataArray[j] = null;
					}
				}
			}
		}

		if (gamepadData.active && gamepadData.prevState.Buttons.A == ButtonState.Released && gamepadData.state.Buttons.A == ButtonState.Pressed)
		{
			stateHandler.menuControl.ToggleReady(gamepadData.playerIndex);
		}
		if (gamepadData.active && gamepadData.prevState.ThumbSticks.Left.X < 0.1f && gamepadData.state.ThumbSticks.Left.X > 0.1f)
		{
			if (stateHandler.menuControl.avatars)
				stateHandler.menuControl.avatars.ChangeAvatar(gamepadData.playerIndex, 1);
		}
		if (gamepadData.active && gamepadData.prevState.ThumbSticks.Left.X > -0.1f && gamepadData.state.ThumbSticks.Left.X < -0.1f)
		{
			if (stateHandler.menuControl.avatars)
				stateHandler.menuControl.avatars.ChangeAvatar(gamepadData.playerIndex, -1);
		}
	}


    public void ConnectToPlayers(List<GameObject> _players)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            Debug.Log("connecting player " + _players[i].GetComponent<Player>().playerNumber);
            Player temp = _players[i].GetComponent<Player>();
            players.Add(temp);
        }       
    }
}
