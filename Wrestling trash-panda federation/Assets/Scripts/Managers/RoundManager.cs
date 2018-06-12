using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoundManager : MonoBehaviour
{
    private float time;
    private float timer;
    private int playersAtStart;
    private int playersDead;

    public UnityEvent OnRoundEnd;
    public bool isOnRound = false;
    public int roundNumber;

    public int PlayersLeft()
    {
        return playersAtStart - playersDead;
    }

    public float TimeLeft()
    {
        return time - timer;
    }

    void StartRound(int _players, int roundNum)
    {
        roundNumber = roundNum;
        timer = 0;
        playersDead = 0;
        playersAtStart = _players;
    }

    public void UpdateRound()
    {
        if (TimeLeft() > 1)
        {
            timer += Time.deltaTime;

        }
        else
        {
            EndRound();
        }
    }

    void EndRound()
    {
        if (roundNumber < 3)
        {
            StartCoroutine(NextRound())
;
        }
        else
        {
            StartCoroutine(EndTimer());
        }
    }

    IEnumerator NextRound()
    {
        //add score
        yield return new WaitForSeconds(2);
        //restore player's positions       
        OnRoundEnd.Invoke();
        StartRound(playersAtStart, roundNumber + 1);
    }

    IEnumerator EndTimer()
    {
        yield return new WaitForSeconds(3);
        //END GAME
    }
}
