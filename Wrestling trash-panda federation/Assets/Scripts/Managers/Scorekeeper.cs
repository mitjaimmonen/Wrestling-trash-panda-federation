using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorekeeper
{
    public int[] scoreArray;

    int winner = 4;
    bool tied = false;
    public int highScore = 0;

    public int PlayerScore(int playerNum)
    {
        return scoreArray[playerNum];
    }

    public Scorekeeper(int playerCount)
    {
        scoreArray = new int[playerCount];

        for (int i = 0; i < scoreArray.Length; i++)
        {
            scoreArray[i] = 0;
        }
    }

    public void AddScore(int value, int playerNum)
    {
        scoreArray[playerNum] += value;
    }

    public void ResetScore()
    {
        scoreArray = null;
    }

    public void OrderScore()
    {
        

        for (int i = 0; i < scoreArray.Length; i++)
        {
            if (scoreArray[i] == highScore)
            {
                tied = true;
                winner = 5;
            }

            else if (scoreArray[i] > highScore)
            {
                highScore = scoreArray[i];
                tied = false;
                winner = i;
            }


        }
    }

    public int ReturnWinner()
    {
        OrderScore();
        return winner;
    }

    public bool Tied()
    {
        OrderScore();
        return tied;

    }

    public int ReturnHighScore()
    {
        OrderScore();
        return highScore;
    }
}
