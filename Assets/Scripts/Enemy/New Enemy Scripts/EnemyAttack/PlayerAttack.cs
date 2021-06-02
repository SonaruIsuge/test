using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using Scream.UniMO;
using UnityEngine;

public class PlayerAttack : EnemyAttackBehavior
{
    private ScaledTimer reloadTimer;
    public PlayerAttack(Enemy parent) : base(parent)
    {
        reloadTimer = new ScaledTimer(parent.property.ReloadTime, false);
    }

    public override void Attack()
    {
        Ray2D ray = new Ray2D(parent.EnemyShootPoint.position, parent.EnemyGun.up);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider && hit.collider.CompareTag("Player"))
        {
            if (reloadTimer.IsFinished)
            {
                parent.shootAnimator.Play("enemyFire");
                EnemyBulletShoot(parent.Bullet, parent.property.BulletSpeed);
                reloadTimer.Reset();
            }
            Debug.DrawLine(ray.origin, hit.point, Color.red);
        }
    }

    private void EnemyBulletShoot(GameObject bullet, float speed)
    {
        var bulletClone = LeanPool.Spawn(bullet, parent.EnemyShootPoint.position, parent.EnemyShootPoint.rotation);
        bulletClone.GetComponent<SpriteRenderer>().color = new Color(0.16f, 0.62f, 0.9f);
        bulletClone.GetComponent<Rigidbody2D>().velocity = parent.EnemyGun.up * speed;
        bulletClone.GetComponent<Bullet>().attack = parent.property.attack;
        bulletClone.GetComponent<Bullet>().Team = parent.Team;
    }
}
