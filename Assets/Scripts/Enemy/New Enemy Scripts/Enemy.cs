using System;
using System.Collections;
using System.Collections.Generic;
using CircleCal.Math;
using Scream.UniMO;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum EnemyState
{
    PatrolState,
    AwareState,
    AttackState,
    DieState
}

public class Enemy : MonoBehaviour
{
    //enemy state
    public EnemyStateMachine stateMachine;

    // enemy properties
    public TankProperty property;
    public Team Team = Team.Enemy;
    public GameObject player;

    //enemy parts
    public SpriteRenderer EnemySprite;
    public GameObject EnemyHead;
    public Transform EnemyGun;
    public Transform EnemyShootPoint;

    //enemy shoot & spawn bullet
    public GameObject Bullet;
    public Animator shootAnimator;
    public ScaledTimer reloadTimer;

    //enemy health
    public int currentHealth;
    public static event Action<EnemyTank, int> EnemyHpChange;

    //enemy patrol
    public PatrolPointControl s_patrolCtrl;
    public Queue<Vector3> PatrolQueue;
    public int segmentNum;
    public Vector3 currentTarget;
    public bool forward = true;

    // enemy path finding
    public Tilemap tilemap;
    public Tilemap colTilemap;
    public float cellSize;
    public PathFinding pathFinding;
    public List<PathNode> path;

    // Enemy Behaviors
    public EnemyMoveBehavior enemyMoveBehavior;
    public EnemyAttackBehavior enemyAttackBehavior;
    public Dictionary<Type, EnemyMoveBehavior> moveBehaviors = new Dictionary<Type, EnemyMoveBehavior>();
    void Awake()
    {
        stateMachine = new EnemyStateMachine(this);
        stateMachine.InitState();

        reloadTimer = new ScaledTimer(property.ReloadTime, false);

        colTilemap.CompressBounds();
        tilemap.CompressBounds();
        pathFinding = new PathFinding(tilemap.cellBounds.size.x, tilemap.cellBounds.size.y, cellSize, tilemap);
        path = new List<PathNode>();

        PatrolQueue = new Queue<Vector3>();
        currentTarget = transform.position;
    }

    void Start()
    {
        // set start state
        stateMachine.ChangeState(EnemyState.PatrolState);
    }

    void Update()
    {
        stateMachine.UpdateState();
    }

    public T GetBehavior<T>(Enemy enemy, Func<Enemy, T>constructor) where T : EnemyMoveBehavior
    {
        Type type = typeof(T);
        if(!moveBehaviors.ContainsKey(type))
        {
            var result = constructor(enemy);
            moveBehaviors.Add(type, result);
        }
        return moveBehaviors[type] as T;
    }

    public float DistanceToPalyer()
    {
        if (player != null)
        {
            return Vector2.Distance(player.transform.position, EnemyHead.transform.position);
        }
        return Mathf.Infinity;
    }

    public void DistroyEnemy()
    {
        Destroy(this.gameObject);
    }

    public float CalAngle(GameObject thisObject, Vector3 targetPos)
    {
        if(targetPos == null) return 0;
        Vector3 thisPos = thisObject.transform.position;
        Vector3 direction = targetPos - thisPos;
        direction.z = 0f;
        direction.Normalize();
        return Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
    }

    public void RotateTarget(GameObject thisObject, Vector3 targetPos, float rotateSpeed)
    {
        float angle = CalAngle(thisObject, targetPos);
        thisObject.transform.rotation = Quaternion.RotateTowards(thisObject.transform.rotation, Quaternion.Euler(0, 0, -angle), rotateSpeed * Time.deltaTime);
    }

    public void MoveTarget(GameObject thisObject, Vector3 targetPos, float moveSpeed)
    {
        thisObject.transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        DrawCircle(EnemyHead.transform.position, property.AttackRange, Color.red);  //攻擊圈(紅)
        DrawCircle(EnemyHead.transform.position, property.ViewRange, Color.green);  //偵查圈(綠)
    }
    
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
}
