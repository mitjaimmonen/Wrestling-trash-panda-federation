using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour {

    public Text[] scoreDisplays = new Text[4];
    public Text round;
    public Text timer;
    public Text countdown;
    public Text victoryText;
    public bool countingDown = false;

    private int PlayerCount()
    {
        return StateHandler.Instance.PlayerCount();
    }

    private float Timer()
    {
        return StateHandler.Instance.roundManager.TimeLeft();
    }

    private int RoundNumber()
    {
        return StateHandler.Instance.roundManager.roundNumber;
    }

    private float Score(int playerNum)
    {
        return StateHandler.Instance.roundManager.getScore(playerNum);
    }

    private bool GameOver()
    {
        return StateHandler.Instance.roundManager.gameEnded;
    }

    private void Start()
    {
        victoryText.enabled = false;
        for (int i = 0; i < scoreDisplays.Length; i++)
        {
            if (i < PlayerCount())
            {
                continue;
            }
            else
            {
                scoreDisplays[i].enabled = false;
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < PlayerCount(); i++)
        {
            scoreDisplays[i].text = "Player " + (i + 1) + "\n " + Score(i);
        }

        timer.text = Timer().ToString("f0");
        round.text = "Round " + RoundNumber();

        if (!countingDown)
            countdown.enabled = false;
        if (GameOver())
        {
            victoryText.text = "Player " + StateHandler.Instance.Winner() + " has achieved ABSOLUTE DOMINATION!";
            victoryText.enabled = true;
        }
    }

   


}
