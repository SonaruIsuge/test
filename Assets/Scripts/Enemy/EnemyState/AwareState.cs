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
            enemy.ChangeState(EnemyTankState.Die);
        }
        if(enemy.DistanceToPalyer() <= enemy.property.AttackRange)
        {
            enemy.ChangeState(EnemyTankState.Attack);
        }
        if(enemy.DistanceToPalyer() > enemy.property.ViewRange)
        {
            enemy.ChangeState(EnemyTankState.Patrol);
        }
    }
    public override void Exit(EnemyTank enemy)
    {
        
    }
}
