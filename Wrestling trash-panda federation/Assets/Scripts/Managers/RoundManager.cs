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
    public Animator fadePanel;

    public UnityEvent OnRoundStart;
    public bool isOnRound = false;
    public bool gameEnded = false;
    public int roundNumber;
    public bool betweenTurns = false;

    public float TimeLeft()
    {
        return startTime - timer;
    }

    public void InitializeRounds(int _players)
    {
        playersAtStart = _players;
        scoreKeeper = new Scorekeeper(_players);
        OnRoundStart.Invoke();
        StartRound(1);
        fadePanel = GameObject.FindGameObjectWithTag("FlashingPanel").GetComponent<Animator>();
    }

    private void StartRound(int roundNum)
    {
        roundNumber = roundNum;
        timer = 0;
        playersDead = 0;


        StartCoroutine(Countdown(3));


        isOnRound = true;
    }

    public void UpdateRound()
    {
        timer += Time.deltaTime;

    }

    private bool ContinueGame()
    {      

        if (roundNumber < 3)
        {
            if (scoreKeeper.ReturnHighScore() < 2)
            {
                return true;
            }            
            else
                return false;
        }

        else
        {
            if (scoreKeeper.Tied())
                return true;
            else
                return false;
        }
    }

    public void EndRound(List<GameObject> playersLeft)
    {
        isOnRound = false;
        if (!gameEnded && !betweenTurns)
        {
            DistributePoints(playersLeft);

            if (ContinueGame())
            {
                StartCoroutine(NextRound());
            }

            else
            {
                gameEnded = true;
                StartCoroutine(EndTimer());

            }
        }

    }

    void DistributePoints(List<GameObject> playersLeft)
    {
        Debug.Log("addin points");
        foreach (GameObject p in playersLeft)
        {
            scoreKeeper.AddScore(1, p.GetComponent<Player>().playerNumber);
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
        betweenTurns = true;
        fadePanel.SetTrigger("Fade");
        //score feedback        
        yield return new WaitForSeconds(0.35f);
        OnRoundStart.Invoke();
        StartRound(roundNumber + 1);
        betweenTurns = false;


    }

    IEnumerator EndTimer()
    {
        yield return new WaitForSeconds(3);
        //END GAME
    }

    public float getScore(int player)
    {
        return scoreKeeper.scoreArray[player];
    }

    public int GetWinner()
    {
        return scoreKeeper.ReturnWinner();
    }
}
