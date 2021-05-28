using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : BaseState
{
    public override void Enter(Enemy enemy)
    {
        enemy.enemyMoveBehavior = enemy.GetBehavior<TracePlayerMove>(enemy, p => new TracePlayerMove(p));
        enemy.enemyAttackBehavior = new PlayerAttack(enemy);
    }
    public override void Stay(Enemy enemy)
    {
        enemy.RotateTarget(enemy.EnemyHead, enemy.player.transform.position, enemy.property.HeadRotSpeed);
        enemy.enemyMoveBehavior?.Move();
        enemy.enemyAttackBehavior?.Attack();
        
        if(enemy.currentHealth <= 0)
        {
            enemy.stateMachine.ChangeState(EnemyState.DieState);
        }
        if(enemy.DistanceToPalyer() > enemy.property.AttackRange)
        {
            enemy.stateMachine.ChangeState(EnemyState.AwareState);
        }
    }
    public override void Exit(Enemy enemy)
    {
        
    }
}
