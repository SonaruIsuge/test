using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState 
{
    public abstract void Enter(Enemy enemy);
    public abstract void Stay(Enemy enemy);
    public abstract void Exit(Enemy enemy);
}