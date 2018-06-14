using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health  {

    int maxHealth;
    int currentHealth;

    public int CurrentHealth
    {
        get { return currentHealth; }
    }

    public Health(int value)
    {
        maxHealth = value;
        SetHealthToMax();
    }
  

    public bool isAlive()
    {
        if (currentHealth > 0)
            return  true;
        else
            return false;
    }

    public void TakeDamage(int value)
    {
        currentHealth -= value;
    }

    public void Heal (int value)
    {
        currentHealth = Mathf.Min(currentHealth+value, maxHealth);
    }

    public void SetHealthToMax()
    {
        currentHealth = maxHealth;
    }


}
