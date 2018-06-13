using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RoundManager : MonoBehaviour
{
    public float startTime = 99;
    private float timer;
    private int playersAtStart;
    private int playersDead;
    private Scorekeeper scoreKeeper;

    public UnityEvent OnRoundStart;
    public bool isOnRound = false;
    public int roundNumber;


    public float TimeLeft()
    {
        return startTime - timer;
    }

    public void InitializeRounds(int _players)
    {
        playersAtStart = _players;
        scoreKeeper = new Scorekeeper(_players);
        StartRound(1);
    }

    private void StartRound(int roundNum)
    {
        roundNumber = roundNum;
        timer = 0;
        playersDead = 0;
        OnRoundStart.Invoke();
       // StartCoroutine(Countdown(3));
        isOnRound = true;
    }

    public void UpdateRound()
    {
        timer += Time.deltaTime;
        Debug.Log("time left: " + TimeLeft());
    }



    public void EndRound(List<GameObject> playersLeft)
    {
        isOnRound = false;

        foreach (GameObject p in playersLeft)
        {
            scoreKeeper.AddScore(1, p.GetComponent<Player>().playerNumber);
        }

        if (roundNumber < 3 && isOnRound)
        {
            StartCoroutine(NextRound());
        }
        else
        {
            StartCoroutine(EndTimer());
        }
    }

    IEnumerator Countdown(float time)
    {
        Time.timeScale = 0;
        //show timer counting down
        yield return new WaitForSeconds(time);
        Time.timeScale = 1;
    }

    IEnumerator NextRound()
    {
        //score feedback
        yield return new WaitForSeconds(2);
        //fade??           
        StartRound(roundNumber + 1);
    }

    IEnumerator EndTimer()
    {
        yield return new WaitForSeconds(3);
        //END GAME
    }
}
