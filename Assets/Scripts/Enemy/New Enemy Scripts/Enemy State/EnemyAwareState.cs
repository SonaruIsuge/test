using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAwareState : BaseState
{
    public override void Enter(Enemy enemy)
    {
        enemy.enemyMoveBehavior = enemy.GetMoveBehavior<RandomMove>(enemy, p => new RandomMove(p));;
        enemy.enemyAttackBehavior = null;
    }
    public override void Stay(Enemy enemy)
    {   
        enemy.RotateTarget(enemy.EnemyHead, enemy.player.transform.position, enemy.property.HeadRotSpeed);
        enemy.enemyMoveBehavior?.Move();

        if(enemy.tankHealth.GetCurrentHealth() <= 0)
        {
            enemy.StateMachine.ChangeState(EnemyState.DieState);
        }
        if(enemy.DistanceToPalyer() <= enemy.property.AttackRange)
        {
            enemy.StateMachine.ChangeState(EnemyState.AttackState);
        }
        if(enemy.DistanceToPalyer() > enemy.property.ViewRange)
        {
            enemy.StateMachine.ChangeState(EnemyState.PatrolState);
        }
    }
    public override void Exit(Enemy enemy)
    {
        
    }
}
