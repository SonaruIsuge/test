using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Scream.UniMO;

public class EnemyHpBar : MonoBehaviour
{
    public Image EnemyHpImg;
    private float healthPercent;
    private bool ImgAlpha = false;
    private float countTime = 5f;
    private ScaledTimer showBarTime;

    // void OnEnable()
    // {
    //     EnemyTank.EnemyHpChange += UpdateEnemyHpBar;
    // }
    //
    // void OnDisable()
    // {
    //     EnemyTank.EnemyHpChange -= UpdateEnemyHpBar;
    // }

    void Awake(){
        SetHpBarAlpha(ImgAlpha);
        showBarTime = new ScaledTimer(countTime, false);
    }

    // Update is called once per frame
    void Update()
    {   
        SetHpBarAlpha(ImgAlpha);
        if(showBarTime.IsFinished)
        {
            ImgAlpha = false;
            showBarTime.Reset();
        }
    }

    void SetHpBarAlpha(bool b)
    {
        if(b)
        {
            EnemyHpImg.GetComponentInParent<CanvasGroup>().alpha = 1;    
        }
        else EnemyHpImg.GetComponentInParent<CanvasGroup>().alpha = 0;
    }

    public void UpdateEnemyHpBar(Enemy sender, int currentHealth)
    {
        showBarTime.Reset();
        ImgAlpha = true;
        healthPercent = currentHealth / (float)sender.property.health;
        EnemyHpImg.fillAmount = healthPercent;
    }
}
