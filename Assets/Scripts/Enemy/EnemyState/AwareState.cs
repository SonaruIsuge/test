using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwareState : State
{
    public override void Enter(EnemyTank enemy)
    {

    }
    public override void Stay(EnemyTank enemy)
    {        
        enemy.LookTarget(enemy.player);
        enemy.CurveMove(true);
        //enemy.TraceTarget(enemy.player);
        if(enemy.currentHealth <= 0)
        {
            enemy.ChangeState(EnemyState.Die);
        }
        if(enemy.DistanceToPalyer() <= enemy.property.AttackRange)
        {
            enemy.ChangeState(EnemyState.Attack);
        }
        if(enemy.DistanceToPalyer() > enemy.property.ViewRange)
        {
            enemy.ChangeState(EnemyState.Patrol);
        }
    }
    public override void Exit(EnemyTank enemy)
    {
        
    }
}
