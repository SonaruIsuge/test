using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float Horizontal {get; private set;}
    public float Vertical {get; private set;}
    public bool FireBullet {get; private set;}

    public event Action OnFire = delegate { };
    // Update is called once per frame
    void Update()
    {
        Horizontal = Input.GetAxis("Horizontal");
        Vertical = Input.GetAxis("Vertical");
        FireBullet = Input.GetMouseButton(0);

        if(FireBullet) OnFire();
    }
}
