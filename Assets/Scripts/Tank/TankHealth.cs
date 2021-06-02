using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TankHealth
{
    private TankProperty property;
    private int currentHealth;

    public TankHealth(TankProperty property)
    {
        this.property = property;
        currentHealth = property.health;
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
    }
}
