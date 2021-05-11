using System;
using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using Scream.UniMO;
using UnityEngine;

public class BulletLauncher : MonoBehaviour
{
    [SerializeField]private TankProperty property;
    [SerializeField]private Transform shootPoint;
    public GameObject Bullet;
    public Animator FireAnimator;
    private ScaledTimer reloadTimer;

    // Start is called before the first frame update
    private void Awake() 
    {
        GetComponent<PlayerInput>().OnFire += HandleFire;
        reloadTimer = new ScaledTimer(property.ReloadTime, false);
    }

    public void HandleFire()
    {
        if (reloadTimer.IsFinished)
        {
            FireAnimator.Play("fire");
            InitBullet();
            reloadTimer.Reset();
        }
    }

    private void InitBullet()
    {
        var BulletClone = LeanPool.Spawn(Bullet, shootPoint.position, shootPoint.rotation);
        BulletClone.GetComponent<SpriteRenderer>().color = new Color(0.17f, 0.7f, 0.32f);
        BulletClone.GetComponent<Rigidbody2D>().AddForce(BulletClone.transform.up * property.BulletSpeed * Time.deltaTime);    //給予砲彈初速
        BulletClone.GetComponent<Bullet>().attack = property.attack;
        BulletClone.GetComponent<Bullet>().Team = 1;    //己方
    }
}
