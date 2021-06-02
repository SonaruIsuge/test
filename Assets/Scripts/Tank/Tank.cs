using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Tank : MonoBehaviour
{
    //tank property
    public TankProperty property;
    [SerializeField]private Team team;
    //tank parts
    [SerializeField]private GameObject tankHead;
    [SerializeField]private Transform tankShootPoint;

    [SerializeField]private Animator fireAnimator;

    [SerializeField]UnityEvent<Tank, int> HandleHpChange;

    private ITankInput tankInput;
    private TankEngine tankEngine;
    private TankHeadRotate tankHeadRotate;
    private TankFire tankFire;
    private TankHealth tankHealth;

    void Awake()
    {
        tankInput = new PlayerControlInput();
        tankEngine = new TankEngine(tankInput, this.transform, property);
        tankHeadRotate = new TankHeadRotate(tankInput, tankHead, property);
        tankFire = new TankFire(tankInput, tankShootPoint, property, fireAnimator, team);
        tankHealth = new TankHealth(property);

        HandleHpChange?.Invoke(this, tankHealth.GetCurrentHealth());
    }

    void Update()
    {
        tankInput.ReadInput();
        tankEngine.Tick();
        tankHeadRotate.Tick();
        tankFire.Tick();
    }

    void OnCollisionEnter2D(Collision2D col) 
    {
        var bullet = col.gameObject.GetComponent<Bullet>();
        if(bullet && bullet.Team != team)
        {
            tankHealth.TakeDamage(bullet.attack);
            HandleHpChange?.Invoke(this, tankHealth.GetCurrentHealth());
            if(tankHealth.GetCurrentHealth() <= 0) Destroy(this.gameObject);
        }
    }
}

