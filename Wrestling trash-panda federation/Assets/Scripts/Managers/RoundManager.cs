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

        StartCoroutine(Countdown(3));
        

        isOnRound = true;
    }

    public void UpdateRound()
    {        
        timer += Time.deltaTime;
        
    }

    public void EndRound(List<GameObject> playersLeft)
    {
        isOnRound = false;

        foreach (GameObject p in playersLeft)
        {
            scoreKeeper.AddScore(1, p.GetComponent<Player>().playerNumber);
        }

        if (roundNumber < 3)
        {
            StartCoroutine(NextRound());
            StartRound(roundNumber + 1);
            Debug.Log("round " + roundNumber + 1);
        }
        else
        {
            StartCoroutine(EndTimer());
        }
    }

    IEnumerator Countdown(float time)
    {
        //players should not
        //show timer counting down
        yield return new WaitForSeconds(3);
        
    }

    IEnumerator NextRound()
    {
        //score feedback        
        yield return new WaitForSeconds(2);
        //fade??           
        
    }

    IEnumerator EndTimer()
    {
        yield return new WaitForSeconds(3);
        //END GAME
    }
}
