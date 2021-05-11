using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    public TankProperty property;
    public int currentHealth;
    private int Team = 1;   //己方

    [SerializeField]UnityEvent<PlayerHealth, int> PlayerHpChange;

    private void Awake() 
    {
        currentHealth = property.health;
        if (PlayerHpChange != null) PlayerHpChange.Invoke(this, currentHealth);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        var bul = collision.gameObject;
        if (bul.gameObject.tag == "Bullet" && Team != bul.gameObject.GetComponent<Bullet>().Team)
        {
            TakeDamage(bul.gameObject.GetComponent<Bullet>().attack);
            PlayerHpChange?.Invoke(this, currentHealth);
        }
    }

    private void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0) Destroy(this.gameObject);
    }
}
