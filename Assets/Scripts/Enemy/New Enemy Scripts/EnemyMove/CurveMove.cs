using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveMove : EnemyMoveBehavior
{
    // public Queue<Vector3> PatrolQueue;
    
    public CurveMove(Enemy parent) :base(parent)
    {

    }
    public override void Move()
    {
        if(parent.transform.position != parent.currentTarget)
        {
            parent.RotateTarget(parent.gameObject, parent.currentTarget, parent.property.RotateSpeed);
            if(Quaternion.Angle(parent.transform.rotation, Quaternion.Euler(0, 0, -parent.CalAngle(parent.gameObject, parent.currentTarget))) <= 3.0f)
            {
                parent.MoveTarget(parent.gameObject ,parent.currentTarget, parent.property.MoveSpeed);
            }
        }
        else 
        {
            if (parent.PatrolQueue.Count == 0)
            {
                //Find New Curve
                if (parent.forward)
                {
                    InitPatrolPoint(parent.transform.position, parent.s_patrolCtrl.points[1], parent.s_patrolCtrl.points[2], parent.s_patrolCtrl.points[3]);
                    parent.forward = !parent.forward;
                    return;
                }
                InitPatrolPoint(parent.transform.position, parent.s_patrolCtrl.points[2], parent.s_patrolCtrl.points[1], parent.s_patrolCtrl.points[0]);
                parent.forward = !parent.forward;
                return;
            }
            parent.currentTarget = parent.PatrolQueue.Dequeue();
        }
    }

    private void InitPatrolPoint(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end)
    {
        if (parent.PatrolQueue.Count == 0)
        {
            parent.PatrolQueue = new Queue<Vector3>();
            for (int i = 1; i <= parent.segmentNum; i++)
            {
                float t = i / (float)parent.segmentNum;
                parent.PatrolQueue.Enqueue(CalBezier(t, start, control1, control2, end));
            }
            parent.currentTarget = parent.PatrolQueue.Dequeue();
        }
    }

    //計算貝茲曲線
    private Vector3 CalBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        Vector3 res = (u * u * u * p0) + (3 * t * u * u * p1) + (3 * t * t * u * p2) + (t * t * t * p3);
        return res;
    }
}

