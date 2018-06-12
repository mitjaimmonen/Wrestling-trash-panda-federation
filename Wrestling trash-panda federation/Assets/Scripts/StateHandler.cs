using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum GameState
{
    menu = 0,
    game = 1
}

public class StateHandler : MonoBehaviour
{

    #region Singleton
    private static StateHandler _instance;
    public static StateHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                return null;
            }

            return _instance;
        }
    }
    #endregion

    public GameState gamestate = GameState.menu;
    public GameObject playerPrefab;

    public delegate void onPlayersChosen();

    public MainMenuController menuControl;
    public GamepadStateHandler gamepadStateHandler;
    public RoundManager roundManager;

    private Mesh[] playerMeshes = new Mesh[4];
    private MeshFilter[] playerMeshFilters = new MeshFilter[4];

    List<Playerdata> playersDatas = new List<Playerdata>();
    List<GameObject> players = new List<GameObject>();


    // Use this for initialization
    void Awake()
    {

        _instance = this;

        if (GameObject.FindGameObjectsWithTag("GameMaster").Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnLevelLoaded;

        Instantiate();

    }

    private void Update()
    {
        if (gamestate == GameState.game)
        {
            if (roundManager.isOnRound)
            {
                roundManager.UpdateRound();
            }
        }
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
            roundManager.enabled = false;
            GameObject menu = GameObject.Find("Main Menu");
            if (menu)
                menuControl = menu.GetComponent<MainMenuController>();

        }
        else if (gamestate == GameState.game)
        {
            SpawnPlayers();
            AttachControllers();
            roundManager.enabled = true;
        }
    }

    private void SpawnPlayers()
    {
        int pDindex = 0;
        foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (pDindex < playersDatas.Count)
            {
                p.GetComponent<Player>().meshNumber = playersDatas[pDindex].meshNumber;
                p.GetComponent<Player>().playerNumber = playersDatas[pDindex].playerIndex;
                players.Add(p);
                Debug.Log("Added player, meshNumber is:" + p.GetComponent<Player>().meshNumber);
                pDindex++;
            }
            else
            {
                p.SetActive(false);
            }

        }

        if (players.Count > 0)
        {

            foreach (GameObject player in players)
            {
                player.GetComponentInChildren<MeshFilter>().mesh = playerMeshes[player.GetComponent<Player>().meshNumber];

            }
        }
    }

    public void ResetAllPlayers()
    {
        foreach (GameObject p in players)
        {
            p.GetComponent<Player>().TransportToStart();
        }
    }

    private void AttachControllers()
    {
        GetComponent<InputHandler>().ConnectToPlayers(players);
    }

    public void SavePlayerData(int amountJoined, Avatars avatars)
    {
        playerMeshes = menuControl.avatars.avatarMeshes;
        playerMeshFilters = menuControl.avatars.avatarMeshFilters;

        for (int i = 0; i < amountJoined; i++)
        {
            Playerdata temp = new Playerdata();
            temp.playerIndex = i;
            temp.meshNumber = avatars.indexes[i];

            playersDatas.Add(temp);
        }
    }
}
