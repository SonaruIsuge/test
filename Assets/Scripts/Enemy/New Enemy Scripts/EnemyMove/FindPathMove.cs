using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FindPathMove : EnemyMoveBehavior
{
    private PathFinding pathFinding;
    private List<PathNode> path;
    private BoundsInt cellBounds, colCellBounds;
    private float MAP_CELL_SIZE = 2f;
    private int startX, startY, endX, endY;

    public FindPathMove(Enemy parent) : base(parent)
    {
        parent.colTilemap.CompressBounds();
        parent.tilemap.CompressBounds();
        cellBounds = parent.tilemap.cellBounds;
        colCellBounds = parent.colTilemap.cellBounds;
        pathFinding = new PathFinding(cellBounds.size.x, cellBounds.size.y, MAP_CELL_SIZE, parent.tilemap);
        SetObstacle();
        path = new List<PathNode>();
        FindPathToCurrentTarget();
    }

    public override void Move()
    {
        //取得grid方塊中間位置
        Vector3 targetPos = pathFinding.GetGrid().GetWorldPosition(path[0].x, path[0].y) + Vector3.one * (MAP_CELL_SIZE/2);  
        
        if(parent.transform.position != targetPos)
        {
            parent.RotateTarget(parent.gameObject, targetPos, parent.property.RotateSpeed);

            //角度容許值：±3°
            if (Quaternion.Angle(parent.transform.rotation, Quaternion.Euler(0, 0, -parent.CalAngle(parent.gameObject, targetPos))) <= 3.0f)
            {
                parent.MoveTarget(parent.gameObject, targetPos, parent.property.MoveSpeed);
            }
        }
        else 
        {
            path.Remove(path[0]);
        }
    }

    private void FindPathToCurrentTarget()
    {
        path.Clear();
        pathFinding.GetGrid().GetXY(parent.transform.position, out startX, out startY);
        pathFinding.GetGrid().GetXY(parent.currentTarget, out endX, out endY);
        path = pathFinding.FindPath(startX, startY, endX, endY);

        for(int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(pathFinding.GetGrid().GetWorldPosition(path[i].x, path[i].y) + Vector3.one * (MAP_CELL_SIZE/2), 
            pathFinding.GetGrid().GetWorldPosition(path[i+1].x, path[i+1].y)+ Vector3.one * (MAP_CELL_SIZE/2), Color.green, 100f);
        }
    }

    private void SetObstacle()
    {
        for(int x = 0; x < cellBounds.size.x; x++)
        {
            for(int y = 0; y < cellBounds.size.y; y++)
            {
                TileBase colTile = parent.colTilemap.GetTile(new Vector3Int(x, y, 0));
                if(colTile != null) 
                {
                    pathFinding.GetNode(x, y)?.SetIsWalkable(false);
                    pathFinding.GetNode(x-1, y)?.SetIsWalkable(false);
                    pathFinding.GetNode(x+1, y)?.SetIsWalkable(false);
                    pathFinding.GetNode(x, y-1)?.SetIsWalkable(false);
                    pathFinding.GetNode(x, y+1)?.SetIsWalkable(false);
                }
            }
        }
    }
}
