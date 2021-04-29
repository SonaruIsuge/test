using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PathFinding {

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14; 

    private GridBase<PathNode> grid;
    private List<PathNode> openList;
    private List<PathNode> closedList;
    public PathFinding(int width, int height, float cellSize, Tilemap tilemap)
    {
        grid = new GridBase<PathNode>(width, height, cellSize, new Vector3(0, 0, 0), (GridBase<PathNode> g, int x, int y) => new PathNode(g, x, y));
        
        tilemap.CompressBounds();
        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);
        
        for(int x = 0; x < bounds.size.x; x++)
        {
            for(int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null) {
                    grid.GetGridObject(x, y).SetTileBase(tile);
                }
            }
        }
    }

    public GridBase<PathNode> GetGrid()
    {
        return grid;
    }

    public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
    {
        PathNode startNode = grid.GetGridObject(startX, startY);
        PathNode endNode = grid.GetGridObject(endX, endY);
        
        // 初始化openList和closeList
        // 將起點加入openList中
        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for(int x = 0; x < grid.GetWidth(); x++)
        {
            for(int y = 0; y < grid.GetHeight(); y++)
            {
                PathNode pathNode = grid.GetGridObject(x, y);
                pathNode.gCost = int.MaxValue;
                pathNode.CalculateFCost();
                pathNode.cameFromNode = null;
            }
        }

        // 設置起點優先級為0（優先級最高）
        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        // 如果openList不為空，則從openList中選取優先級最高的節點currentNode
        while(openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);

            // 如果currentNode為終點，則
            if(currentNode == endNode)
            {
                // 從終點開始逐步追蹤parent節點，一直達到起點，返回找到的結果路徑，算法結束
                return CalculatePath(endNode);
            }
            
            // 如果currentNode不是終點，則將currentNode從openList中刪除，並加入closeList中
            openList.Remove(currentNode);
            closedList.Add(currentNode);

            // 遍歷currentNode所有的鄰近節點
            foreach(PathNode neighborNode in GetNeighborList(currentNode))
            {
                // 如果鄰近節點m在closeList中，則跳過，選取下一個鄰近節點
                if(closedList.Contains(neighborNode)) continue;

                if(!neighborNode.isWalkable)
                {
                    closedList.Add(neighborNode);
                    continue;
                }
                
                // 設置節點m的parent為currentNode
                // 計算節點m的優先級
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighborNode);
                if(tentativeGCost < neighborNode.gCost)
                {
                    neighborNode.cameFromNode = currentNode;
                    neighborNode.gCost = tentativeGCost;
                    neighborNode.hCost = CalculateDistanceCost(neighborNode, endNode);
                    neighborNode.CalculateFCost();
                    
                    // 如果鄰近節點m也不在openList中，則將節點m加入openList中
                    if(!openList.Contains(neighborNode))
                    {
                        openList.Add(neighborNode);
                    }
                }
            }
        }

        //Out of nodes on the openList
        return null;
    }

    private List<PathNode> GetNeighborList(PathNode currentNode)
    {
        List<PathNode> neighborList = new List<PathNode>();

        if(currentNode.x - 1 >= 0)
        {
            //Left
            neighborList.Add(GetNode(currentNode.x - 1, currentNode.y));
            //LeftDown
            if(currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x - 1, currentNode.y - 1));
            //LeftUp
            if(currentNode.y + 1 < grid.GetHeight()) neighborList.Add(GetNode(currentNode.x - 1, currentNode.y + 1));
        }
        if(currentNode.x + 1 < grid.GetWidth())
        {
            //Right
            neighborList.Add(GetNode(currentNode.x + 1, currentNode.y));
            //RightDown
            if(currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x + 1, currentNode.y - 1));
            //RightUp
            if(currentNode.y + 1 < grid.GetHeight()) neighborList.Add(GetNode(currentNode.x + 1, currentNode.y + 1));
        }
        //Down
        if(currentNode.y - 1 >= 0) neighborList.Add(GetNode(currentNode.x, currentNode.y - 1));
        //Up
        if(currentNode.y + 1 < grid.GetHeight()) neighborList.Add(GetNode(currentNode.x, currentNode.y + 1));

        return neighborList;
    }

    public PathNode GetNode(int x, int y)
    {
        return grid.GetGridObject(x, y);
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
       List<PathNode> path = new List<PathNode>();
       path.Add(endNode);
       PathNode currentNode = endNode;
       while(currentNode.cameFromNode != null)
       {
           path.Add(currentNode.cameFromNode);
           currentNode = currentNode.cameFromNode;
       }
       path.Reverse();
       return path;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b)
    {
        int xDis = Mathf.Abs(a.x - b.x);
        int yDis = Mathf.Abs(a.y - b.y);
        int remaining = Mathf.Abs(xDis - yDis);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDis, yDis) + MOVE_STRAIGHT_COST * remaining;
    }

    private PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];
        for(int i = 1; i < pathNodeList.Count; i++)
        {
            if(pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }
        return lowestFCostNode;
    }
}