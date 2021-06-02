using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine
{
    private Enemy enemy;
    private BaseState CurrentState;
    private Dictionary<EnemyState, BaseState> StateDic;

    public EnemyStateMachine(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public void InitState()
    {
        StateDic = new Dictionary<EnemyState, BaseState>()
        {
            {EnemyState.PatrolState, new EnemyPatrolState()},
            {EnemyState.AwareState, new EnemyAwareState()},
            {EnemyState.AttackState, new EnemyAttackState()},
            {EnemyState.DieState, new EnemyDieState()}
        };
    }

    public void UpdateState()
    {
        CurrentState.Stay(enemy);
    }

    public void ChangeState(EnemyState newState)
    {
        CurrentState?.Exit(enemy);
        if(StateDic[newState] != null) CurrentState = StateDic[newState];
        CurrentState?.Enter(enemy);
    }

}
