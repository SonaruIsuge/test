using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : BaseState
{
    public override void Enter(Enemy enemy)
    {
        enemy.enemyMoveBehavior = enemy.GetBehavior<RandomMove>(enemy, p => new RandomMove(p));
        // enemy.enemyMoveBehavior = new RandomMove(enemy);
        enemy.enemyAttackBehavior = null;
    }
    public override void Stay(Enemy enemy)
    {
        if(enemy.currentTarget != enemy.transform.position) enemy.RotateTarget(enemy.EnemyHead, enemy.currentTarget, enemy.property.HeadRotSpeed);
        enemy.enemyMoveBehavior?.Move();

        if(enemy.currentHealth <= 0)
        {
            enemy.stateMachine.ChangeState(EnemyState.DieState);
        }
        if(enemy.DistanceToPalyer() <= enemy.property.ViewRange)
        {
            enemy.stateMachine.ChangeState(EnemyState.AwareState);
        }
    }
    public override void Exit(Enemy enemy)
    {
        
    }
}
