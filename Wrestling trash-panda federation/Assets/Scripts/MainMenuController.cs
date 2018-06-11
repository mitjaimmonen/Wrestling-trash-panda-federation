using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

	public Text[] joinTexts = new Text[4];
	public Text[] readyTexts = new Text[4];
	[HideInInspector]public bool[] ready = new bool[4];

	// Use this for initialization
	void Awake () {
		for(int i = 0; i < ready.Length; i++)
			ready[i] = false;
	}

	public void ToggleReady(int playerNumber)
	{
		if (!ready[playerNumber])
		{
			readyTexts[playerNumber].color = Color.green;
			ready[playerNumber] = true;
		}
		else
		{
			readyTexts[playerNumber].color = Color.white;
			ready[playerNumber] = false;

		}
	}
	public void ToggleReady(int playerNumber, bool state)
	{
		if (state)
		{
			readyTexts[playerNumber].color = Color.green;
			ready[playerNumber] = state;
		}
		else
		{
			readyTexts[playerNumber].color = Color.white;
			ready[playerNumber] = state;
		}

	}

	public void AddPlayer(int playerNumber)
	{
		readyTexts[playerNumber].gameObject.SetActive(true);
		joinTexts[playerNumber].text = "Joined!";
		ToggleReady(playerNumber, false);
	}
	public void RemovePlayer(int playerNumber)
	{
		readyTexts[playerNumber].gameObject.SetActive(false);
		joinTexts[playerNumber].text = "Join";
		ToggleReady(playerNumber, false);
		
	}
}
