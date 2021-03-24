using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGrid : MonoBehaviour
{
    private GridBase<HeatMapGridObject> grid;
    public Camera mainCamera;

    // Start is called before the first frame update
    void Start()
    {
        grid = new GridBase<HeatMapGridObject>(4, 2, 10f, Vector3.zero, (GridBase<HeatMapGridObject> g, int x, int y) => new HeatMapGridObject(g, x, y));
    }

    // Update is called once per frame
    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            HeatMapGridObject heatMapGridObject = grid.GetGridObject(worldPosition);
            if(heatMapGridObject != null) heatMapGridObject.AddValue(5);
        }

        if(Input.GetMouseButtonDown(1))
        {
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(grid.GetGridObject(worldPosition));
        }
    }

}


public class HeatMapGridObject{
    private const int MIN = 0;
    private const int MAX = 100;
    
    private GridBase<HeatMapGridObject> grid;
    private int x;
    private int y;
    private int value;

    public HeatMapGridObject(GridBase<HeatMapGridObject> grid, int x, int y)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public void AddValue(int addValue)
    {
        value += addValue;
        Mathf.Clamp(value, MIN, MAX);
        grid.TriggerGrridObjectChanged(x, y);
    }

    public float GetValueNormalize()
    {
        return (float)value / MAX;
    }

    public override string ToString()
    {
        return value.ToString();
    }
}