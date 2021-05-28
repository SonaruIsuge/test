using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using Scream.UniMO;
using UnityEngine;

public class TankFire 
{
    private ITankInput tankInput;
    private Transform tankShootPoint;
    private TankProperty property;
    private Animator fireAnimator;
    private ScaledTimer reloadTimer;
    private GameObject Bullet;
    private Team team;
    
    public TankFire(ITankInput tankInput, Transform tankShootPoint, TankProperty property, Animator fireAnimator, Team team)
    {
        this.tankInput = tankInput;
        this.tankShootPoint = tankShootPoint;
        this.property = property;
        this.fireAnimator = fireAnimator;
        this.team = team;

        reloadTimer = new ScaledTimer(property.ReloadTime, false);
        Bullet =  (GameObject)Resources.Load("prefabs/Bullet", typeof(GameObject));
    }

    public void Tick()
    {
        if(tankInput.FireBullet)
        {
            if (reloadTimer.IsFinished)
            {
                fireAnimator.Play("fire");
                InitBullet();
                reloadTimer.Reset();
            }
        }
    }

    private void InitBullet()
    {
        var BulletClone = LeanPool.Spawn(Bullet, tankShootPoint.position, tankShootPoint.rotation);
        BulletClone.GetComponent<SpriteRenderer>().color = new Color(0.17f, 0.7f, 0.32f);
        BulletClone.GetComponent<Rigidbody2D>().AddForce(BulletClone.transform.up * property.BulletSpeed * Time.deltaTime);    //給予砲彈初速
        BulletClone.GetComponent<Bullet>().attack = property.attack;
        BulletClone.GetComponent<Bullet>().Team = team;    //己方
    }
}
