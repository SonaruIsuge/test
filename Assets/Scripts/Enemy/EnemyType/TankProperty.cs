﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TankType", menuName = "Create Type")]
public class TankProperty : ScriptableObject
{
    [Header("About")]
    public int Id;
    public TankTypes types;
    public string Name;
    public Sprite MiniMapIcon;

    [Header("Weaponry")]
    public int attack;
    public float BulletSpeed;
    public float ReloadTime;

    [Header("Armor")]
    public int health;

    [Header("Mobility")]
    public float MoveSpeed;
    public float RotateSpeed;   //how much radius per second(use to * Time.daltaTime)
    public float HeadRotSpeed;
    
    [Header("Investigation")]
    public int AttackRange;
    public int ViewRange;
}

public enum TankTypes
{
    LightTank,
    NormalTank,
    HeavyTank

}