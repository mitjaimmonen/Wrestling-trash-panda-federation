using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
	menu = 0,
	game = 1
}

public class StateHandler : MonoBehaviour {


	public GameState gamestate = GameState.menu;
	public GameObject playerPrefab;

	public MainMenuController menuControl;
	public GamepadStateHandler gamepadStateHandler;

	List<Player> players = new List<Player>();


	// Use this for initialization
	void Awake () {

		if (GameObject.FindGameObjectsWithTag("GameMaster").Length > 1)
		{
			Destroy(this.gameObject);
		}

		DontDestroyOnLoad(this.gameObject);
		SceneManager.sceneLoaded += OnLevelLoaded;

		Instantiate();

	}

	void OnDisable()
	{
		SceneManager.sceneLoaded -= OnLevelLoaded;
		
	}

	void OnLevelLoaded(Scene scene, LoadSceneMode mode)
	{
		//Gets called on sceneload
		gamestate = scene.buildIndex == 1 ? GameState.game : GameState.menu;
		Instantiate();
	}


	void Instantiate()
	{

		if (!gamepadStateHandler)
			gamepadStateHandler = GetComponent<GamepadStateHandler>();

		if (gamestate == GameState.menu)
		{
			GameObject menu = GameObject.Find("Main Menu");
			if (menu)
				menuControl = menu.GetComponent<MainMenuController>();
			
		}
		else if (gamestate == GameState.game)
		{
			foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
			{
				players.Add (p.GetComponent<Player>());
			}
		}
	}
}
