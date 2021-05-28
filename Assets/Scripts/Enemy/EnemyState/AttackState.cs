using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    public override void Enter(EnemyTank enemy)
    {
        
    }
    public override void Stay(EnemyTank enemy)
    {
        //enemy.RotateTarget(enemy.gameObject, enemy.player, enemy.property.RotateSpeed);
        enemy.RotateTarget(enemy.EnemyHead, enemy.player, enemy.property.HeadRotSpeed, out float angle);
        enemy.TraceTarget(enemy.player);
        enemy.ShootTarget();
        
        if(enemy.currentHealth <= 0)
        {
            enemy.ChangeState(EnemyTankState.Die);
        }
        if(enemy.DistanceToPalyer() > enemy.property.AttackRange)
        {
            enemy.ChangeState(EnemyTankState.Aware);
        }
    }
    public override void Exit(EnemyTank enemy)
    {
        enemy.FindPathToCurrentTarget();
    }
}
