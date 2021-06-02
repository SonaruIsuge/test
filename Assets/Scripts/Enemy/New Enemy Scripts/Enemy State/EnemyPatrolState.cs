using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : BaseState
{
    public override void Enter(Enemy enemy)
    {
        enemy.enemyMoveBehavior = enemy.GetMoveBehavior<RandomMove>(enemy, p => new RandomMove(p));
        enemy.enemyAttackBehavior = null;
    }
    public override void Stay(Enemy enemy)
    {
        if(enemy.currentTarget != enemy.transform.position) enemy.RotateTarget(enemy.EnemyHead, enemy.currentTarget, enemy.property.HeadRotSpeed);
        enemy.enemyMoveBehavior?.Move();

        if(enemy.tankHealth.GetCurrentHealth() <= 0)
        {
            enemy.StateMachine.ChangeState(EnemyState.DieState);
        }
        if(enemy.DistanceToPalyer() <= enemy.property.ViewRange)
        {
            enemy.StateMachine.ChangeState(EnemyState.AwareState);
        }
    }
    public override void Exit(Enemy enemy)
    {
        
    }
}
