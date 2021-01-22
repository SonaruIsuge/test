using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircleCal.Math;
using Eccentric.Utils;
using Lean.Pool;

public enum EnemyState 
{
    Patrol,
    Aware,
    Attack,
    Die
}

public class EnemyTank : MonoBehaviour
{
    //當前狀態
    public State CurrentState;

    public GameObject EnemyHead;
    public Transform EnemyGun;
    public Transform EnemyShootPoint;
    public GameObject Bullet;
    private GameObject BulletClone;
    public TankProperty property;
    public GameObject player;
    private ScaledTimer reloadTimer;
    public int currentHealth;
    public int Team = 0;
    private Rigidbody2D enemyRb;

    public GameObject PatrolCtrlObj;
    public Transform[] BezierCtrlPt;
    public Queue<Vector3> PatrolQueue;
    public int segmentNum;
    public Vector3 currentTarget;

    public Dictionary<EnemyState, State> StateDic;

    
    void Awake()
    {   
        StateDic =new Dictionary<EnemyState, State>()
        {
            {EnemyState.Patrol, new PatrolState()},
            {EnemyState.Aware, new AwareState()},
            {EnemyState.Attack, new AttackState()},
            {EnemyState.Die, new DieState()}
        };     
        currentHealth = property.health;
        player = FindObjectOfType<Player>().gameObject;
        enemyRb = GetComponent<Rigidbody2D>();
        PatrolQueue = new Queue<Vector3>();
    }

    void Start() 
    {
        CurrentState = StateDic[EnemyState.Patrol];
        CurrentState.Enter(this);
        reloadTimer = new ScaledTimer(property.ReloadTime, false);
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState.Stay(this);
        //Debug.Log(CurrentState);
    }

    //設定扣血
    void OnCollisionEnter2D(Collision2D bul)
    {
        if (bul.gameObject.tag == "Bullet" && Team != bul.gameObject.GetComponent<Bullet>().Team)
        {
            currentHealth -= bul.gameObject.GetComponent<Bullet>().attack;
        }
    }

    //偵錯用(畫圓)
    void OnDrawGizmos()
	{
		DrawCircle(EnemyHead.transform.position, property.AttackRange, Color.red);  //攻擊圈(紅)
        DrawCircle(EnemyHead.transform.position, property.ViewRange, Color.green);  //偵查圈(綠)
	}

    //改變狀態
    public void ChangeState(EnemyState newState)
    {
        CurrentState.Exit(this);
        CurrentState = StateDic[newState];
        CurrentState.Enter(this);
    }

    //計算與玩家間的距離
    public float DistanceToPalyer()
    {
        if(player != null)
        {
            return Vector2.Distance(player.transform.position, EnemyHead.transform.position);
        }
        return Mathf.Infinity;
    }

    //敵人巡邏模式移動
    public void CurveMove()
    {    
        
        if(transform.position!=currentTarget)
        {
            Vector3 dir = currentTarget - transform.position;
            dir.z = 0f;
            dir.Normalize();
            float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, -angle), property.RotateSpeed * Time.deltaTime);
            if(transform.rotation == Quaternion.Euler(0, 0, -angle))
            {
                transform.position = Vector3.MoveTowards(transform.position, currentTarget, property.MoveSpeed * Time.deltaTime);
            }
        }
        else
        {
            if(PatrolQueue.Count == 0)
            {
                //Find New Curve
                
                return;
            }
            currentTarget = PatrolQueue.Dequeue();
        }
    }

    //敵人瞄準目標
    public void LookTarget(GameObject target)
    {
        if(target != null)
        {
            Vector3 targetPos = target.transform.position;
            Vector3 direction = targetPos - EnemyHead.transform.position;
            direction.z = 0f;
            direction.Normalize();
            float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
            EnemyHead.transform.rotation = Quaternion.RotateTowards(EnemyHead.transform.rotation, Quaternion.Euler(0, 0, -angle), property.HeadRotSpeed * Time.deltaTime);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, -angle), property.RotateSpeed * Time.deltaTime);
        }
    }

    //敵人射擊
    public void ShootTarget()
    {
        Ray2D ray = new Ray2D(EnemyShootPoint.position, EnemyGun.up);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
        if (hit.collider && hit.collider.tag == "Player")
        {
            if (reloadTimer.IsFinished)
            {
                EnemyBulletShoot(Bullet, property.BulletSpeed);
                reloadTimer.Reset();
            }
            Debug.DrawLine(ray.origin,hit.point, Color.red );
        }
    }

    //設定子彈生成與前進
    private void EnemyBulletShoot(GameObject bullet, float speed)
    {
        Vector3 pos = EnemyShootPoint.position;
        Quaternion rot = EnemyShootPoint.rotation;
        BulletClone = LeanPool.Spawn(bullet, pos, rot);
        BulletClone.GetComponent<SpriteRenderer>().color = new Color(0.16f, 0.62f, 0.9f);
        BulletClone.GetComponent<Rigidbody2D>().velocity = EnemyGun.up * speed;
        BulletClone.GetComponent<Bullet>().attack = property.attack;
        BulletClone.GetComponent<Bullet>().Team = Team;
    }

    //消滅
    public void DestoryTank()
    {
        Destroy(this.gameObject);
    }

    //生成巡邏點
    public void InitPatrolPoint(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end)
    {
        if(PatrolQueue.Count == 0)
        {
            PatrolQueue = new Queue<Vector3>();
            for(int i = 0; i < segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                PatrolQueue.Enqueue(CalBezier(t, start, control1, control2, end));
            }
            currentTarget = PatrolQueue.Dequeue();
        }
    }

    //計算貝茲曲線(巡邏狀態)
    public Vector3 CalBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        Vector3 res = (u * u * u * p0) + (3 * t * u * u * p1) + (3 * t * t * u * p2) + (t * t * t * p3);
        return res;
    }

    //偵錯用(畫攻擊範圍、視野範圍)
    [ExecuteInEditMode]
    public void DrawCircle(Vector2 _center, float _radius, Color _color)
    {
        Circle2d circle = new Circle2d(_center, _radius);
        int count = 40;
        float delta = (2f * Mathf.PI) / count;
        Vector2 prev = circle.Eval(0);

        //Color tempColor = Gizmos.color;
		Gizmos.color = _color;

        for(int i = 0; i <= count; i++)
        {
            Vector3 curr = circle.Eval(i * delta);
            Gizmos.DrawLine(prev, curr);
            prev = curr;
        }

        //Gizmos.color = tempColor;
    }

}
