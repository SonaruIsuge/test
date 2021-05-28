using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CircleCal.Math;
using Scream.UniMO;
using Lean.Pool;
using System;
using UnityEngine.Tilemaps;

public enum EnemyTankState
{
    Patrol,
    Aware,
    Attack,
    Die
}


public enum Team
{
    Enemy,
    Player,
    Other
}

public class EnemyTank : MonoBehaviour
{
    //當前狀態
    public State CurrentState;

    public SpriteRenderer EnemySprite;
    public GameObject EnemyHead;
    public Transform EnemyGun;
    public Transform EnemyShootPoint;
    public GameObject Bullet;
    private GameObject BulletClone;
    public TankProperty property;
    public GameObject player;
    private ScaledTimer reloadTimer;
    public int currentHealth;
    public Team Team = Team.Enemy;

    public GameObject PatrolCtrlObj;
    public PatrolPointControl s_patrolCtrl;
    public Queue<Vector3> PatrolQueue;
    public int segmentNum;
    public Vector3 currentTarget;
    private bool forward = true;

    public Animator animator;

    public Tilemap tilemap;
    public Tilemap colTilemap;
    public float cellSize;
    private PathFinding pathFinding;
    private int startX, startY, endX, endY;
    private List<PathNode> path;

    public Dictionary<EnemyTankState, State> StateDic;

    public static event Action<EnemyTank, int> EnemyHpChange;


    void Awake()
    {
        StateDic = new Dictionary<EnemyTankState, State>()
        {
            {EnemyTankState.Patrol, new PatrolState()},
            {EnemyTankState.Aware, new AwareState()},
            {EnemyTankState.Attack, new AttackState()},
            {EnemyTankState.Die, new DieState()}
        };

        currentHealth = property.health;
        EnemySprite.sprite = property.MiniMapIcon;
        player = GameObject.FindWithTag("Player");
        PatrolQueue = new Queue<Vector3>();
        s_patrolCtrl = PatrolCtrlObj.GetComponent<PatrolPointControl>();
        currentTarget = transform.position;

        colTilemap.CompressBounds();
        tilemap.CompressBounds();
        pathFinding = new PathFinding(tilemap.cellBounds.size.x, tilemap.cellBounds.size.y, cellSize, tilemap);
        path = new List<PathNode>();
    }

    void Start()
    {
        // set start state
        CurrentState = StateDic[EnemyTankState.Patrol];
        CurrentState.Enter(this);
        // init fire reload timer
        reloadTimer = new ScaledTimer(property.ReloadTime, false);
    }

    // Update is called once per frame
    void Update()
    {
        CurrentState.Stay(this);
    }

    //設定扣血
    void OnCollisionEnter2D(Collision2D bul)
    {
        if (bul.gameObject.tag == "Bullet" && Team != bul.gameObject.GetComponent<Bullet>().Team)
        {
            currentHealth -= bul.gameObject.GetComponent<Bullet>().attack;

            if(EnemyHpChange != null) EnemyHpChange(this, currentHealth);
        }
    }

    //偵錯用(畫圓)
    void OnDrawGizmos()
    {
        DrawCircle(EnemyHead.transform.position, property.AttackRange, Color.red);  //攻擊圈(紅)
        DrawCircle(EnemyHead.transform.position, property.ViewRange, Color.green);  //偵查圈(綠)
    }

    //改變狀態
    public void ChangeState(EnemyTankState newState)
    {
        CurrentState.Exit(this);
        CurrentState = StateDic[newState];
        CurrentState.Enter(this);
    }

    //計算與玩家間的距離
    public float DistanceToPalyer()
    {
        if (player != null)
        {
            return Vector2.Distance(player.transform.position, EnemyHead.transform.position);
        }
        return Mathf.Infinity;
    }

    //敵人巡邏模式移動
    public void CurveMove(bool isAware)
    {
        if(path != null && path.Count > 0)
        {
            MoveToPath();
        }
        else if (transform.position != currentTarget)
        {
            float angle;
            if(!isAware) RotateTarget(this.EnemyHead, currentTarget, property.HeadRotSpeed, out float headAngle);
            RotateTarget(this.gameObject, currentTarget, property.RotateSpeed, out angle);
            //Debug.Log(angle);
            //角度容許值：±3°
            if (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 0, -CalAngle(this.gameObject, currentTarget))) <= 3.0f)
            {
                MoveTarget(this.gameObject, currentTarget, property.MoveSpeed);
            }
        }
        else
        {
            if (PatrolQueue.Count == 0)
            {
                //Find New Curve
                if (forward)
                {
                    InitPatrolPoint(transform.position, s_patrolCtrl.points[1], s_patrolCtrl.points[2], s_patrolCtrl.points[3]);
                    forward = !forward;
                    return;
                }
                InitPatrolPoint(transform.position, s_patrolCtrl.points[2], s_patrolCtrl.points[1], s_patrolCtrl.points[0]);
                forward = !forward;
                return;
            }
            currentTarget = PatrolQueue.Dequeue();
        }
    }

    public void MoveToPath()
    {
        Vector3 targetPos = pathFinding.GetGrid().GetWorldPosition(path[0].x, path[0].y) + Vector3.one * (cellSize/2);  //取得grid方塊中間位置
        if(transform.position != targetPos)
        {
            RotateTarget(this.EnemyHead, targetPos, property.HeadRotSpeed, out float headAngle);
            RotateTarget(this.gameObject, targetPos, property.RotateSpeed, out float angle);

            //角度容許值：±3°
            if (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 0, -angle)) <= 3.0f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, property.MoveSpeed * Time.deltaTime);
            }
        }
        else 
        {
            path.Remove(path[0]);                
        }
    }

    //敵人砲塔瞄準目標
    public void LookTarget(GameObject target)
    {
        if (target != null)
            RotateTarget(this.EnemyHead, target, property.HeadRotSpeed, out float angle);
    }

    //尋找到currentTarget的最短路徑
    public void FindPathToCurrentTarget()
    {
        path.Clear();
        pathFinding.GetGrid().GetXY(this.transform.position, out startX, out startY);
        pathFinding.GetGrid().GetXY(currentTarget, out endX, out endY);
        SetObstacle();
        path = pathFinding.FindPath(startX, startY, endX, endY);

        for(int i = 0; i < path.Count - 1; i++)
        {
            Debug.DrawLine(new Vector3(path[i].x, path[i].y) * cellSize + Vector3.one * (cellSize/2) + path[i].grid.originPosition, 
            new Vector3(path[i+1].x, path[i+1].y) * cellSize + Vector3.one * (cellSize/2) + path[i].grid.originPosition, Color.green, 100f);
        }
    }

    //敵人追擊
    public void TraceTarget(GameObject target)
    {
        if(target == null) return;
        
        Vector3 targetPos = target.transform.position;
        RotateTarget(this.gameObject, targetPos, property.RotateSpeed, out float angle);
        
        //角度容許值：±3°
        if (Quaternion.Angle(transform.rotation, Quaternion.Euler(0, 0, -angle)) <= 3.0f && DistanceToPalyer() > 5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, property.MoveSpeed * Time.deltaTime);
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
                animator.Play("enemyFire");
                EnemyBulletShoot(Bullet, property.BulletSpeed);
                reloadTimer.Reset();
            }
            Debug.DrawLine(ray.origin, hit.point, Color.red);
        }
    }

    //設定子彈生成與前進
    private void EnemyBulletShoot(GameObject bullet, float speed)
    {
        BulletClone = LeanPool.Spawn(bullet, EnemyShootPoint.position, EnemyShootPoint.rotation);
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
        if (PatrolQueue.Count == 0)
        {
            // PatrolQueue = new Queue<Vector3>();
            for (int i = 1; i <= segmentNum; i++)
            {
                float t = i / (float)segmentNum;
                PatrolQueue.Enqueue(CalBezier(t, start, control1, control2, end));
                
            }
            currentTarget = PatrolQueue.Dequeue();
        }
    }

    //計算貝茲曲線(巡邏狀態)
    private Vector3 CalBezier(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
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

        for (int i = 0; i <= count; i++)
        {
            Vector3 curr = circle.Eval(i * delta);
            Gizmos.DrawLine(prev, curr);
            prev = curr;
        }

        //Gizmos.color = tempColor;
    }

    //設定pathfinding 障礙物
    public void SetObstacle()
    {
        for(int x = 0; x < tilemap.cellBounds.size.x; x++)
        {
            for(int y = 0; y < tilemap.cellBounds.size.y; y++)
            {
                TileBase colTile = colTilemap.GetTile(new Vector3Int(x, y, 0));
                if(colTile != null) 
                {
                    pathFinding.GetNode(x, y)?.SetIsWalkable(false);
                    pathFinding.GetNode(x-1, y)?.SetIsWalkable(false);
                    pathFinding.GetNode(x+1, y)?.SetIsWalkable(false);
                    pathFinding.GetNode(x, y-1)?.SetIsWalkable(false);
                    pathFinding.GetNode(x, y+1)?.SetIsWalkable(false);
                }
            }
        }
    }

    //定速轉到定位
    public void RotateTarget(GameObject thisObject, Vector3 targetPos, float rotateSpeed, out float angle)
    {
        if(targetPos == null) {angle = 0; return;}
        Vector3 thisPos = thisObject.transform.position;
        Vector3 direction = targetPos - thisPos;
        direction.z = 0f;
        direction.Normalize();
        angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        thisObject.transform.rotation = Quaternion.RotateTowards(thisObject.transform.rotation, Quaternion.Euler(0, 0, -angle), rotateSpeed * Time.deltaTime);
    }

    public void RotateTarget(GameObject thisObject, GameObject target, float rotateSpeed, out float angle)
    {
        if(target == null) {angle = 0; return;}
        Vector3 targetPos = target.transform.position;
        RotateTarget(thisObject, targetPos, rotateSpeed, out angle);
    }

    private float CalAngle(GameObject thisObject, Vector3 targetPos)
    {
        if(targetPos == null) return 0;
        Vector3 thisPos = thisObject.transform.position;
        Vector3 direction = targetPos - thisPos;
        direction.z = 0f;
        direction.Normalize();
        return Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
    }

    public void MoveTarget(GameObject thisObject, Vector3 targetPos, float moveSpeed)
    {
        thisObject.transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }
}
