using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

	public Text[] joinTexts = new Text[4];
	public Text[] readyTexts = new Text[4];
	public Text countdownText;
	[HideInInspector]public bool[] ready = new bool[4];

	int amountJoined = 0;
	int amountReady = 0;
	bool isCountdown = false;

	// Use this for initialization
	void Awake () {
		for(int i = 0; i < ready.Length; i++)
			ready[i] = false;
	}

	void Update()
	{
		if (amountJoined == amountReady && amountJoined > 1 && !isCountdown)
		{
			isCountdown = true;
			StartCoroutine(StartCountdown());
		}
		if (isCountdown && amountJoined != amountReady)
			isCountdown = false;
	}

	public void LoadLevel(string levelName)
	{
		SceneManager.LoadScene(levelName);
	}

	IEnumerator StartCountdown()
	{
		countdownText.gameObject.SetActive(true);
		int countdownTime = 5;

		while (isCountdown && countdownTime >= 0)
		{
			countdownText.text = countdownTime.ToString();
			yield return new WaitForSeconds(1);
			countdownTime--;
			if (countdownTime < 0)
				isCountdown = false;
		}
		if (countdownTime < 0)
		{
			LoadLevel("Level01");
		}
		countdownText.gameObject.SetActive(false);
		yield break;
	}

	public void ToggleReady(int playerNumber)
	{
		if (!ready[playerNumber])
		{
			readyTexts[playerNumber].color = Color.green;
			ready[playerNumber] = true;
			amountReady++;
		}
		else
		{
			readyTexts[playerNumber].color = Color.white;
			ready[playerNumber] = false;
			amountReady--;
		}
	}
	public void ToggleReady(int playerNumber, bool state)
	{
		if (state)
		{
			if(!ready[playerNumber]) //Add to ready only if toggle true
				amountReady++;
			readyTexts[playerNumber].color = Color.green;
			ready[playerNumber] = state;
		}
		else
		{
			if(ready[playerNumber]) //Remove from ready only if toggle true
				amountReady--;
			readyTexts[playerNumber].color = Color.white;
			ready[playerNumber] = state;
		}

	}

	public void AddPlayer(int playerNumber)
	{
		readyTexts[playerNumber].gameObject.SetActive(true);
		joinTexts[playerNumber].text = "Joined!";
		ToggleReady(playerNumber, false);
		amountJoined++;
	}
	public void RemovePlayer(int playerNumber)
	{
		readyTexts[playerNumber].gameObject.SetActive(false);
		joinTexts[playerNumber].text = "Join";
		ToggleReady(playerNumber, false);
		amountJoined--;
	}
}
