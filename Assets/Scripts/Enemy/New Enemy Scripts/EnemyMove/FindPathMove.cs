using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FindPathMove : EnemyMoveBehavior
{
    private float MAP_CELL_SIZE = 2f;
    private int startX, startY, endX, endY;

    public FindPathMove(Enemy parent) : base(parent)
    {
        SetObstacle();
        FindPathToCurrentTarget();
    }

    public override void Move()
    {
        //取得grid方塊中間位置
        Vector3 targetPos = parent.pathFinding.GetGrid().GetWorldPosition(parent.path[0].x, parent.path[0].y) + Vector3.one * (MAP_CELL_SIZE/2);  
        
        if(parent.transform.position != targetPos)
        {
            parent.RotateTarget(parent.gameObject, targetPos, parent.property.RotateSpeed);

            //角度容許值：±3°
            if (Quaternion.Angle(parent.transform.rotation, Quaternion.Euler(0, 0, -parent.CalAngle(parent.gameObject, parent.currentTarget))) <= 3.0f)
            {
                parent.MoveTarget(parent.gameObject, targetPos, parent.property.MoveSpeed);
            }
        }
        else 
        {
            parent.path.Remove(parent.path[0]);
        }
    }

    public void FindPathToCurrentTarget()
    {
        parent.path.Clear();
        parent.pathFinding.GetGrid().GetXY(parent.transform.position, out startX, out startY);
        parent.pathFinding.GetGrid().GetXY(parent.currentTarget, out endX, out endY);
        parent.path = parent.pathFinding.FindPath(startX, startY, endX, endY);

        for(int i = 0; i < parent.path.Count - 1; i++)
        {
            Debug.DrawLine(parent.pathFinding.GetGrid().GetWorldPosition(parent.path[i].x, parent.path[i].y) + Vector3.one * (MAP_CELL_SIZE/2), 
            parent.pathFinding.GetGrid().GetWorldPosition(parent.path[i+1].x, parent.path[i+1].y)+ Vector3.one * (MAP_CELL_SIZE/2), Color.green, 100f);
        }
    }

    public void SetObstacle()
    {
        for(int x = 0; x < parent.tilemap.cellBounds.size.x; x++)
        {
            for(int y = 0; y < parent.tilemap.cellBounds.size.y; y++)
            {
                TileBase colTile = parent.colTilemap.GetTile(new Vector3Int(x, y, 0));
                if(colTile != null) 
                {
                    parent.pathFinding.GetNode(x, y)?.SetIsWalkable(false);
                    parent.pathFinding.GetNode(x-1, y)?.SetIsWalkable(false);
                    parent.pathFinding.GetNode(x+1, y)?.SetIsWalkable(false);
                    parent.pathFinding.GetNode(x, y-1)?.SetIsWalkable(false);
                    parent.pathFinding.GetNode(x, y+1)?.SetIsWalkable(false);
                }
            }
        }
    }
}
