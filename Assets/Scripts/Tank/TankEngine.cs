using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankEngine 
{
    private ITankInput tankInput;
    private Transform tank;
    private TankProperty property;
    public TankEngine(ITankInput tankInput, Transform tank, TankProperty property)
    {
        this.tankInput = tankInput;
        this.tank = tank;
        this.property = property;
    }

    public void Tick()
    {
        tank.position += tankInput.Vertical * tank.up * property.MoveSpeed * Time.deltaTime;
        tank.Rotate(-Vector3.forward * tankInput.Horizontal * property.RotateSpeed * Time.deltaTime);
    }
}
