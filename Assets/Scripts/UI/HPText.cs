using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPText : MonoBehaviour
{
    public Text HpText;

    // void OnEnable()
    // {
    //     Player.PlayerHpChange += UpdateHpText;
    // }

    // void OnDisable()
    // {
    //     Player.PlayerHpChange -= UpdateHpText;
    // }


    public void UpdateHpText(Tank sender, int currentHealth)
    {
        if (currentHealth <= 0)
        {
            HpText.text = "0";
        }
        else
        {
            HpText.text = currentHealth.ToString();
        }
    }
}
