using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracePlayerMove : EnemyMoveBehavior
{
    private Transform player;
    public TracePlayerMove(Enemy parent) : base(parent)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    public override void Move()
    {
        if(parent.player == null) return;

        parent.RotateTarget(parent.gameObject, player.position, parent.property.RotateSpeed);
        if (Quaternion.Angle(parent.transform.rotation, Quaternion.Euler(0, 0, -parent.CalAngle(parent.gameObject, player.position))) <= 3.0f && parent.DistanceToPalyer() > 5f)
        {
            parent.MoveTarget(parent.gameObject, player.position, parent.property.MoveSpeed);
        }
    }
}
