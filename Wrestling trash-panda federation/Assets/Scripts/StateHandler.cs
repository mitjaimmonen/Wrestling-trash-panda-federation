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
    public bool roundTest;

    public delegate void onPlayersChosen();

    public MainMenuController menuControl;
    public GamepadStateHandler gamepadStateHandler;
    public RoundManager roundManager;

    private Mesh[] playerMeshes = new Mesh[4];
    private MeshFilter[] playerMeshFilters = new MeshFilter[4];

    List<Playerdata> playersDatas = new List<Playerdata>();
    List<GameObject> players = new List<GameObject>();

    public List<GameObject> LeftPlayers()
    {
        List<GameObject> temp = new List<GameObject>();

        foreach (GameObject p in players)
        {
            if (p.gameObject.activeSelf)
            {
                temp.Add(p);
            }
        }

        return temp;
    }



    // Use this for initialization
    void Awake()
    {


        if (GameObject.FindGameObjectsWithTag("GameMaster").Length > 1)
        {
            Destroy(this.gameObject);
        }

        _instance = this;

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

            else if (LeftPlayers().Count < 2 || roundManager.TimeLeft()<=0)
            {
                roundManager.EndRound(LeftPlayers());
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
        if (roundTest)
        {
            gamestate = scene.buildIndex != 0 ? GameState.game : GameState.menu;
        }
        else
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
            roundManager.InitializeRounds(players.Count);
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
