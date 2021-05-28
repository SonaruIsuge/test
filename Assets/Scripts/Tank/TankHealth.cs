using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TankHealth
{
    private TankProperty property;
    private int currentHealth;
    private Team team;

    public TankHealth(TankProperty property, Team team)
    {
        this.property = property;
        this.team = team;
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
