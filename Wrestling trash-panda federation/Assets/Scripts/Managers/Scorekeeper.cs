using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scorekeeper {

    public int[] scoreArray;

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
	
}
