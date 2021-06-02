using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : BaseState
{
    public override void Enter(Enemy enemy)
    {
        enemy.enemyMoveBehavior = enemy.GetMoveBehavior<TracePlayerMove>(enemy, p => new TracePlayerMove(p));
        enemy.enemyAttackBehavior = enemy.GetAttackBehavior<PlayerAttack>(enemy, p => new PlayerAttack(p));
    }
    public override void Stay(Enemy enemy)
    {
        enemy.RotateTarget(enemy.EnemyHead, enemy.player.transform.position, enemy.property.HeadRotSpeed);
        enemy.enemyMoveBehavior?.Move();
        enemy.enemyAttackBehavior?.Attack();
        
        if(enemy.tankHealth.GetCurrentHealth() <= 0)
        {
            enemy.StateMachine.ChangeState(EnemyState.DieState);
        }
        if(enemy.DistanceToPalyer() > enemy.property.AttackRange)
        {
            enemy.StateMachine.ChangeState(EnemyState.AwareState);
        }
    }
    public override void Exit(Enemy enemy)
    {
        
    }
}
