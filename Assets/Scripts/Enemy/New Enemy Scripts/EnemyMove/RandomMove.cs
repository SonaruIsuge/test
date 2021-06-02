using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMove : EnemyMoveBehavior
{
    private float MIN_DISTANCE =  1.5f;
    private float RANDOM_LENGTH = 10f;
    private float OBETACLE_DETECT_LENGTH = 4f;
    private int rayAmount = 5;
    public RandomMove(Enemy parent) : base(parent)
    {
        
    }

    public override void Move()
    {
        
        DebugRay();

        if(NeedNewDestination())
        {
            GetNewDestination();
        }
        parent.RotateTarget(parent.gameObject, parent.currentTarget, parent.property.RotateSpeed);
        if(Quaternion.Angle(parent.transform.rotation, Quaternion.Euler(0, 0, -parent.CalAngle(parent.gameObject, parent.currentTarget))) <= 3.0f)
        {
            if(IsPathBlocked())
            {
                GetNewDestination();
                return;
            }
            parent.MoveTarget(parent.gameObject , parent.currentTarget, parent.property.MoveSpeed);
        }        
    }

    private bool NeedNewDestination()
    {
        if(parent.currentTarget == Vector3.zero) return true;

        var distance = Vector3.Distance(parent.transform.position, parent.currentTarget);
        if(distance <= MIN_DISTANCE) return true;

        return false;
    }

    private void GetNewDestination()
    {
        var transform = parent.transform;
        Vector3 randomPos = transform.position + transform.up * 4f + 
                            new Vector3(Random.Range(RANDOM_LENGTH, -RANDOM_LENGTH), Random.Range(RANDOM_LENGTH, -RANDOM_LENGTH), 0);
        parent.currentTarget = randomPos;
    }

    private bool IsPathBlocked()
    {
        Vector3 leftStart = Quaternion.Euler(0, 0, -45) * parent.transform.up * OBETACLE_DETECT_LENGTH;
        for(int i = 0; i < rayAmount; i++)
        {
            Vector3 vector = Quaternion.Euler(0, 0, (90 / (rayAmount-1)) * i) * leftStart;
            Ray2D ray = new Ray2D(parent.transform.position, vector);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, OBETACLE_DETECT_LENGTH , 1<<8);
            if(hit.collider) return true;
        }
        return false;
    }

    private void DebugRay()
    {
        Debug.DrawLine(parent.transform.position, parent.currentTarget, Color.blue);
        Vector3 leftStart = Quaternion.Euler(0, 0, -45) * parent.transform.up * OBETACLE_DETECT_LENGTH;
        for(int i = 0; i < rayAmount; i++)
        {
            Vector3 vector = Quaternion.Euler(0, 0, (90 / (rayAmount-1)) * i) * leftStart;
            Ray2D ray = new Ray2D(parent.transform.position, vector);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, OBETACLE_DETECT_LENGTH, 1<<8);
            if(hit.collider) Debug.Log(hit.collider.name);
            Debug.DrawRay(ray.origin, vector, hit.collider ? Color.red : Color.green);
        }
    }
}
