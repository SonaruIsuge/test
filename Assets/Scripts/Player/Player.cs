using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Lean.Pool;
using System;
using UnityEngine.Events;

public class Player : MonoBehaviour
{
    public GameObject Head;
    public Transform Gun;
    public Transform ShootPoint;
    public GameObject Bullet;
    private GameObject BulletClone;
    public TankProperty property;
    protected float CountTime = 0;
    public int Team = 1;
    public int currentHealth;

    //public static event Action<Player, int> PlayerHpChange;
    [SerializeField]UnityEvent<Player, int> PlayerHpChange;

    void Start() => InitPlayerHealth();

    void InitPlayerHealth()
    {
        currentHealth = property.health;
        if (PlayerHpChange != null)
            // PlayerHpChange(this, currentHealth);
            PlayerHpChange.Invoke(this, currentHealth);
    }

    void FixedUpdate()
    {
        Move();
        HeadControl();
        Shoot();
        DestoryTank();
    }

    //血量控制
    void OnCollisionEnter2D(Collision2D bul)
    {
        if (bul.gameObject.tag == "Bullet" && Team != bul.gameObject.GetComponent<Bullet>().Team)
        {
            currentHealth -= bul.gameObject.GetComponent<Bullet>().attack;
            //if (PlayerHpChange != null) PlayerHpChange(this, currentHealth);
            PlayerHpChange?.Invoke(this, currentHealth);
        }
    }

    public void Move()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector2.up * property.MoveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector2.down * property.MoveSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * property.RotateSpeed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.forward * Time.deltaTime * -property.RotateSpeed);
        }
    }

    public void HeadControl()
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
        Vector3 direction = worldPos - Head.transform.position;
        direction.z = 0f;
        direction.Normalize();
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        Head.transform.rotation = Quaternion.RotateTowards(Head.transform.rotation, Quaternion.Euler(0, 0, -angle), property.HeadRotSpeed * Time.deltaTime);
    }
    public void Shoot()
    {
        CountTime += Time.deltaTime;
        if (Input.GetMouseButton(0))
        {
            if (CountTime >= property.ReloadTime)
            {
                InitBullet();
                CountTime = 0;
            }
        }
    }
    private void InitBullet()
    {
        Vector3 pos = ShootPoint.transform.position;
        Quaternion rot = ShootPoint.transform.rotation;
        BulletClone = LeanPool.Spawn(Bullet, pos, rot);
        BulletClone.GetComponent<SpriteRenderer>().color = new Color(0.17f, 0.7f, 0.32f);
        BulletClone.GetComponent<Rigidbody2D>().velocity = Gun.up * property.BulletSpeed;//給予砲彈初速
        BulletClone.GetComponent<Bullet>().attack = property.attack;
        BulletClone.GetComponent<Bullet>().Team = Team;
    }

    void DestoryTank()
    {
        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
