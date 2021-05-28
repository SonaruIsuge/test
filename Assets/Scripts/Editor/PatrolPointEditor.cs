using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class PatrolPointEditor : EditorWindow {

    private GameObject pointObject;
    private Vector3[] points = new Vector3[4];

    Vector3 mousePosition;
    Ray ray;
    private bool startPointSet, ctrlPoint1Set, ctrlPoint2Set, endPointSet;
    private List<GameObject> patrolObject;
    private int index;
    private GameObject selectPatrolObject;

    [MenuItem("Custom Editor/PatrolPoint")]
    private static void ShowWindow() {
        var window = GetWindow<PatrolPointEditor>();
        window.titleContent = new GUIContent("PatrolPoint");
        window.Show();
    }

    void OnEnable() 
    {
        SceneView.duringSceneGui += OnSceneGUI;
        pointObject = (GameObject)Resources.Load("prefabs/Point", typeof(GameObject));
    }

    void OnDisable() 
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnGUI() 
    {
        GUILayout.Label("Selection GameObject");
        if(Selection.activeGameObject?.GetComponent<PatrolPointControl>())
        {
            selectPatrolObject = Selection.activeGameObject;
            PrefabUtility.RecordPrefabInstancePropertyModifications(selectPatrolObject.GetComponent<PatrolPointControl>());
            points = selectPatrolObject.GetComponent<PatrolPointControl>().points;
            // for(int i = 0; i < selectPatrolObject.GetComponent<PatrolPointControl>().points.Length; i++)
            // {
            //     points[i] = selectPatrolObject.GetComponent<PatrolPointControl>().points[i];
            // }
        }

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.ObjectField("Select", selectPatrolObject, typeof(GameObject), true);
        EditorGUI.EndDisabledGroup();

        if(selectPatrolObject == null)
        {
            Repaint();
            return;
        }
        
        points[0] = EditorGUILayout.Vector3Field("Start Point", points[0]);
        startPointSet = GUILayout.Toggle(startPointSet, "Set Start Point", "Button");
        if(startPointSet)
        {
            selectPatrolObject.GetComponent<PatrolPointControl>().points[0] = new Vector3(ray.origin.x, ray.origin.y, 0);
            EditorUtility.SetDirty(selectPatrolObject);
        }

        EditorGUILayout.Space();

        points[1] = EditorGUILayout.Vector3Field("Control Point 1", points[1]);
        ctrlPoint1Set = GUILayout.Toggle(ctrlPoint1Set, "Set Control Point 1", "Button");
        if(ctrlPoint1Set)
        {
            selectPatrolObject.GetComponent<PatrolPointControl>().points[1] = new Vector3(ray.origin.x, ray.origin.y, 0);
            EditorUtility.SetDirty(selectPatrolObject);
        }

        EditorGUILayout.Space();

        points[2] = EditorGUILayout.Vector3Field("Control Point 2", points[2]);
        ctrlPoint2Set = GUILayout.Toggle(ctrlPoint2Set, "Set Control Point 2", "Button");
        if(ctrlPoint2Set)
        {
            selectPatrolObject.GetComponent<PatrolPointControl>().points[2] = new Vector3(ray.origin.x, ray.origin.y, 0);
            EditorUtility.SetDirty(selectPatrolObject);
        }

        EditorGUILayout.Space();

        points[3] = EditorGUILayout.Vector3Field("End Point", points[3]);
        endPointSet = GUILayout.Toggle(endPointSet, "Set End Point", "Button");
        if(endPointSet)
        {
            selectPatrolObject.GetComponent<PatrolPointControl>().points[3] = new Vector3(ray.origin.x, ray.origin.y, 0);
            EditorUtility.SetDirty(selectPatrolObject);
        }

        EditorGUILayout.Space();

        if(GUILayout.Button("Clear All"))
        {
            for(int i = 0; i < points.Length; i++)
            {
                points[i] = Vector3.zero;
                EditorUtility.SetDirty(selectPatrolObject);
            }
        }

        Repaint();
    }

    private void OnSceneGUI(SceneView sceneView) 
    {
        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            mousePosition = Event.current.mousePosition;
            float ppp = EditorGUIUtility.pixelsPerPoint;
            mousePosition.y = sceneView.camera.pixelHeight - mousePosition.y * ppp; // Flip y
            mousePosition.x *= ppp;
            ray = sceneView.camera.ScreenPointToRay(mousePosition);
            Debug.Log(ray);
        }
    }

}