using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankHeadRotate
{
    private ITankInput tankInput;
    private GameObject tankHead;
    private TankProperty property;
    public TankHeadRotate(ITankInput tankInput, GameObject tankHead, TankProperty property)
    {
        this.tankInput = tankInput;
        this.tankHead = tankHead;
        this.property = property;
    }

    public void Tick()
    {
        Vector3 direction = tankInput.LookTarget - tankHead.transform.position;
        direction.z = 0f;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        tankHead.transform.rotation = Quaternion.RotateTowards(tankHead.transform.rotation, Quaternion.Euler(0, 0, -angle), property.HeadRotSpeed * Time.deltaTime);
    }
}
