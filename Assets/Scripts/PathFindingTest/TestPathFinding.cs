using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestPathFinding : MonoBehaviour
{
    public Camera mainCamera;
    public PathFinding pathFinding;

    public int width;
    public int height;
    public float cellSize;

    public Tilemap groundTilemap;
    public Tilemap colliderTilemap;

    private int startX, startY, endX, endY;
    private  List<PathNode> path;
    [SerializeField]private PFState currentState;
    
    void Awake() 
    {
        colliderTilemap.CompressBounds();
        groundTilemap.CompressBounds();
    }

    void Start()
    {
        width = groundTilemap.cellBounds.size.x;
        height = groundTilemap.cellBounds.size.y;
        pathFinding = new PathFinding(width, height, cellSize, groundTilemap);
        currentState = PFState.SET_START;
        SetObstacle();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateObstacleTiles();

        if(Input.GetMouseButtonDown(0))
        {   
            switch(currentState)
            {
                case PFState.SET_START:
                    GetMouseWorldPosition(out startX, out startY);
                    if(startX >= 0 && startY >= 0 && startX < pathFinding.GetGrid().GetWidth() && startY < pathFinding.GetGrid().GetHeight())
                        currentState = PFState.SET_END;
                    break;
                case PFState.SET_END:
                    GetMouseWorldPosition(out endX, out endY);
                    if(endX >= 0 && endY >= 0 && endX < pathFinding.GetGrid().GetWidth() && endY < pathFinding.GetGrid().GetHeight())
                        currentState = PFState.FIND_PATH;
                    break;
                default:
                    break;
            }

            if(currentState == PFState.FIND_PATH)
            {
                path = pathFinding.FindPath(startX, startY, endX, endY);
                currentState = PFState.SET_START;
            }

            if(path != null)
            {
                for(int i = 0; i < path.Count - 1; i++)
                {
                    Debug.DrawLine(new Vector3(path[i].x, path[i].y) * cellSize + Vector3.one * (cellSize/2) + path[i].grid.originPosition, 
                    new Vector3(path[i+1].x, path[i+1].y) * cellSize + Vector3.one * (cellSize/2) + path[i].grid.originPosition, Color.green, 100f);
                }
            }            
        }
    }

    public void GetMouseWorldPosition(out int x, out int y)
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        pathFinding.GetGrid().GetXY(mouseWorldPosition, out x, out y);
        Debug.Log("(" + x + " , " + y + ")");
    }

    public void SetObstacle()
    {
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                TileBase colTile = colliderTilemap.GetTile(new Vector3Int(x, y, 0));
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

    public void UpdateObstacleTiles()
    {
        for(int x = 0; x < pathFinding.GetGrid().GetWidth(); x++)
        {
            for(int y = 0; y < pathFinding.GetGrid().GetHeight(); y++)
            {
                if(pathFinding.GetNode(x, y).isWalkable == false)
                    pathFinding.GetGrid().debugTextArray[x, y].color = Color.gray;
                else pathFinding.GetGrid().debugTextArray[x, y].color = Color.blue;
            }
        }
    }

    private enum PFState
    {
        SET_START,
        SET_END,
        FIND_PATH
    };
}
