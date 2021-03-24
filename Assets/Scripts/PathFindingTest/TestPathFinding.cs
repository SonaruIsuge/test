using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPathFinding : MonoBehaviour
{
    public Camera mainCamera;
    public PathFinding pathFinding;

    private int startX, startY, endX, endY;
    private bool isSetStart = false;
    private bool isSetEnd = false;

    void Start()
    {
        pathFinding = new PathFinding(10, 10);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {   
            if(!isSetStart)
            {
                Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                pathFinding.GetGrid().GetXY(mouseWorldPosition, out startX, out startY);
                foreach(TextMesh t in pathFinding.GetGrid().debugTextArray)
                {
                    t.color = Color.white;
                }
                pathFinding.GetGrid().debugTextArray[startX, startY].color = Color.red;
                isSetStart = true;
            }
            else if(isSetStart && !isSetEnd)
            {
                Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                pathFinding.GetGrid().GetXY(mouseWorldPosition, out endX, out endY);
                
                pathFinding.GetGrid().debugTextArray[endX, endY].color = Color.blue;
                isSetStart = false;
                List<PathNode> path = pathFinding.FindPath(startX, startY, endX, endY);
                if(pathFinding != null)
                {
                    for(int i = 0; i < path.Count - 1; i++)
                    {
                        Debug.DrawLine(new Vector3(path[i].x, path[i].y) * 10f + Vector3.one * 5f + path[i].grid.originPosition, 
                        new Vector3(path[i+1].x, path[i+1].y) * 10f + Vector3.one * 5f + path[i].grid.originPosition, Color.green, 100f);
                    }
                }
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            pathFinding.GetGrid().GetXY(mouseWorldPosition, out int x, out int y);
            pathFinding.GetNode(x, y).SetIsWalkable(!pathFinding.GetNode(x, y).isWalkable);
            pathFinding.GetGrid().debugTextArray[x, y].color = Color.gray;
        }
    }
}
