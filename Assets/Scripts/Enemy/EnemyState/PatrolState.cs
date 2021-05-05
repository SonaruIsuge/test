using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    public override void Enter(EnemyTank enemy)
    {
        //enemy.currentTarget  = enemy.transform.position;
    }
    public override void Stay(EnemyTank enemy)
    {
        enemy.CurveMove(false);

        if(enemy.currentHealth <= 0)
        {
            enemy.ChangeState(EnemyState.Die);
        }
        if(enemy.DistanceToPalyer() <= enemy.property.ViewRange)
        {
            enemy.ChangeState(EnemyState.Aware);
        }
    }
    public override void Exit(EnemyTank enemy)
    {
        
    }
}
