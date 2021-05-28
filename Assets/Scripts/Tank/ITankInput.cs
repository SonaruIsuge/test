
using System;
using UnityEngine;

public interface ITankInput
{
    float Horizontal {get; }
    float Vertical {get; }

    Vector3 LookTarget {get; }

    bool FireBullet {get; }
    
    void ReadInput();
}
