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
			gamepadData = HandleMenuInputs(gamepadData);
		}
		if (stateHandler.gamestate == GameState.game)
		{
			players[gamepadData.characterIndex].HandleInput(gamepadData.state, gamepadData.prevState);
		}

		return gamepadData;
	}

	public PlayerGamepadData HandleFixedInput(PlayerGamepadData gamepadData)
	{
		if (stateHandler.gamestate == GameState.game)
		{
			players[gamepadData.characterIndex].HandleFixedInput(gamepadData.state, gamepadData.prevState);
		}
		return gamepadData;
	}
	

	PlayerGamepadData HandleMenuInputs(PlayerGamepadData gamepadData)
	{
		//Handle player (gamepad) activation - check if Y-button was pressed in this gamepad in this frame
		if (gamepadData.prevState.Buttons.Y == ButtonState.Released && gamepadData.state.Buttons.Y == ButtonState.Pressed)
		{
			//Invert active bool
			Debug.Log("GAMEPAD characterIndex:" + gamepadData.characterIndex + ", gamepadPlayerIndex: " + gamepadData.gamepadPlayerIndex + ", active: " + gamepadData.active);
			gamepadData.active = !gamepadData.active;
			Debug.Log("After toggling active state, Gamepad active: "+gamepadData.active);
			if (gamepadData.active)
			{
				Playerdata newPlayerdata = new Playerdata();

				//Index should be first null
				for(int j = 0; j < gamepad.playerDataArray.Length; j++)
				{
					if (gamepad.playerDataArray[j] == null)
					{
						Debug.Log("Activated gamepad's characterIndex: " + j);
						newPlayerdata.characterIndex = j;
						break;
					}
				}

				gamepadData.characterIndex = newPlayerdata.characterIndex;
				newPlayerdata.gamepadPlayerIndex = gamepadData.gamepadPlayerIndex;

				for(int j = 0; j < gamepad.playerDataArray.Length; j++)
				{
					if (gamepad.playerDataArray[j] == null)
					{
						gamepad.playerDataArray[j] = newPlayerdata;
						Debug.Log("Added to array");
						break;
					}
				}
					
				stateHandler.menuControl.AddPlayer(newPlayerdata.characterIndex);
				
			}
			else
			{
				for (int j = 0; j < gamepad.playerDataArray.Length; j++)
				{
					if (gamepad.playerDataArray[j] != null && gamepad.playerDataArray[j].gamepadPlayerIndex == gamepadData.gamepadPlayerIndex)
					{
						stateHandler.menuControl.RemovePlayer(gamepad.playerDataArray[j].characterIndex);
						Debug.Log("deactivation of gamepad: " + gamepad.playerDataArray[j].gamepadPlayerIndex + ", characterIndex: " + gamepad.playerDataArray[j].characterIndex);
						gamepad.playerDataArray[j] = null;
					}
				}
			}
		}

		if (gamepadData.active && gamepadData.prevState.Buttons.A == ButtonState.Released && gamepadData.state.Buttons.A == ButtonState.Pressed)
		{
			stateHandler.menuControl.ToggleReady(gamepadData.characterIndex);
		}
		if (gamepadData.active && gamepadData.prevState.ThumbSticks.Left.X < 0.1f && gamepadData.state.ThumbSticks.Left.X > 0.1f)
		{
			if (stateHandler.menuControl.avatars)
				stateHandler.menuControl.avatars.ChangeAvatar(gamepadData.characterIndex, 1);
		}
		if (gamepadData.active && gamepadData.prevState.ThumbSticks.Left.X > -0.1f && gamepadData.state.ThumbSticks.Left.X < -0.1f)
		{
			if (stateHandler.menuControl.avatars)
				stateHandler.menuControl.avatars.ChangeAvatar(gamepadData.characterIndex, -1);
		}
		return gamepadData;
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
