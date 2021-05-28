using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDieState : BaseState
{
    public override void Enter(Enemy enemy)
    {
        enemy.enemyMoveBehavior = null;
        enemy.enemyAttackBehavior = null;
    }
    public override void Stay(Enemy enemy)
    {
        enemy.DistroyEnemy();
    }
    public override void Exit(Enemy enemy)
    {
        
    }
}
