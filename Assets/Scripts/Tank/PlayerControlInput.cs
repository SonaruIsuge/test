using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlInput : ITankInput
{
    public float Horizontal {get; private set;}
    public float Vertical {get; private set;}
    public Vector3 LookTarget {get; private set;}
    public bool FireBullet {get; private set;}

    public event Action OnFire = delegate{ };

    public void ReadInput()
    {
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");
        LookTarget = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        FireBullet = Input.GetMouseButton(0);

        if(FireBullet) OnFire();
    }
}
