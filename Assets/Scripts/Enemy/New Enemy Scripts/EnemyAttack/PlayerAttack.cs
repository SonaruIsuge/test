using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using Scream.UniMO;
using UnityEngine;

public class PlayerAttack : EnemyAttackBehavior
{
    public PlayerAttack(Enemy parent) : base(parent)
    {
        
    }

    public override void Attack()
    {
        Ray2D ray = new Ray2D(parent.EnemyShootPoint.position, parent.EnemyGun.up);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider && hit.collider.tag == "Player")
        {
            if (parent.reloadTimer.IsFinished)
            {
                parent.shootAnimator.Play("enemyFire");
                EnemyBulletShoot(parent.Bullet, parent.property.BulletSpeed);
                parent.reloadTimer.Reset();
            }
            Debug.DrawLine(ray.origin, hit.point, Color.red);
        }
    }

    private void EnemyBulletShoot(GameObject bullet, float speed)
    {
        var BulletClone = LeanPool.Spawn(bullet, parent.EnemyShootPoint.position, parent.EnemyShootPoint.rotation);
        BulletClone.GetComponent<SpriteRenderer>().color = new Color(0.16f, 0.62f, 0.9f);
        BulletClone.GetComponent<Rigidbody2D>().velocity = parent.EnemyGun.up * speed;
        BulletClone.GetComponent<Bullet>().attack = parent.property.attack;
        BulletClone.GetComponent<Bullet>().Team = parent.Team;
    }
}
