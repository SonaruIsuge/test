using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EnemyTank))]
[CanEditMultipleObjects]
public class EnemyInspector : Editor
{   
    bool showParts = false;
    bool showPatrolSettings = false;
    EnemyTank enemy;
    void OnEnable()
    {
        //獲取當前編輯自定義Inspector的物件
        enemy = (EnemyTank)target;
    }

     public override void OnInspectorGUI()
     {

        //設定整個介面是以垂直方向來佈局
        EditorGUILayout.BeginVertical();
        
        enemy.property = (TankProperty)EditorGUILayout.ObjectField("Enemy Property", enemy.property, typeof(TankProperty), true);

        enemy.currentHealth = EditorGUILayout.IntSlider("Enemy Health", enemy.currentHealth, 0, enemy.property.health);

        enemy.Team = EditorGUILayout.IntField("Enemy Team", enemy.Team);

        switch(enemy.Team)
        {
            case 0:
                EditorGUILayout.HelpBox("Default Enemy Team", MessageType.Info);
                break;
            case 1:
                EditorGUILayout.HelpBox("Player Team", MessageType.Warning);
                break;
            default:
                EditorGUILayout.HelpBox("Team" + enemy.Team, MessageType.Info);
                break;
        }

        EditorGUILayout.Space();

        showParts = EditorGUILayout.Foldout(showParts, "Parts");
        if(showParts)
        {
            enemy.EnemyHead = (GameObject)EditorGUILayout.ObjectField("Enemy Head", enemy.EnemyHead, typeof(GameObject), true);
            enemy.EnemyGun = (Transform)EditorGUILayout.ObjectField("Enemy Gun", enemy.EnemyGun, typeof(Transform), true);
            enemy.EnemyShootPoint = (Transform)EditorGUILayout.ObjectField("Enemy Shoot Point", enemy.EnemyShootPoint, typeof(Transform), true);
            enemy.Bullet = (GameObject)EditorGUILayout.ObjectField("Bullet", enemy.Bullet, typeof(GameObject), true);
            enemy.EnemySprite = (SpriteRenderer)EditorGUILayout.ObjectField("SpriteRenderer", enemy.EnemySprite, typeof(SpriteRenderer), true);
        }

        showPatrolSettings = EditorGUILayout.Foldout(showPatrolSettings, "Patrol Settings");
        if(showPatrolSettings)
        {
            enemy.PatrolCtrlObj = (GameObject)EditorGUILayout.ObjectField("Control Point", enemy.PatrolCtrlObj, typeof(GameObject), true);
            enemy.segmentNum = EditorGUILayout.IntField("Segment Number", enemy.segmentNum);
            enemy.currentTarget = EditorGUILayout.Vector3Field("Current Target", enemy.currentTarget);
        }
        
        EditorGUILayout.Space();    

        enemy.animator = (Animator)EditorGUILayout.ObjectField("Animator", enemy.animator, typeof(Animator), true);

        EditorGUILayout.EndVertical();

        if (GUI.changed)
            EditorUtility.SetDirty(enemy);
     }
}
