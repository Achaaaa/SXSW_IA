using UnityEngine;
using UnityEditor;
using System;

public class CreateBeamLightSculpture : EditorWindow
{
    private GameObject parent;
    private GameObject prefab;
    private int num = 5;
    private float interval = 1;

    [MenuItem("Custom/Create Bream Lights")]
    static void Init()
    {
        EditorWindow.GetWindow<CreateBeamLightSculpture>(true, "Create Bream Lights");
    }

    void OnEnable()
    {
        if (Selection.gameObjects.Length > 0) parent = Selection.gameObjects[0];
    }

    void OnSelectionChange()
    {
        if (Selection.gameObjects.Length > 0) prefab = Selection.gameObjects[0];
        Repaint();
    }

    void OnGUI()
    {
        try
        {
            parent = EditorGUILayout.ObjectField("Parent", parent, typeof(GameObject), true) as GameObject;
            prefab = EditorGUILayout.ObjectField("Beam Lights", prefab, typeof(GameObject), true) as GameObject;

            GUILayout.Label("Beam Lights : ", EditorStyles.boldLabel);
            num = int.Parse(EditorGUILayout.TextField("num", num.ToString()));
            interval = float.Parse(EditorGUILayout.TextField("interval", interval.ToString()));

            GUILayout.Label("", EditorStyles.boldLabel);
            if (GUILayout.Button("Create")) Create();
        }
        catch (System.FormatException) { }
    }

    private void Create()
    {
        if (parent == null || prefab == null) return;

        // parent objects
        parent.name = "BeamLightsSculpture";
        parent.AddComponent(typeof(Light));
        var targetLight = parent.GetComponent<Light>();
        targetLight.enabled = false;

        GameObject beamLights = new GameObject("BeamLights");
        GameObject targets = new GameObject("Targets");
        beamLights.transform.parent = parent.transform;
        beamLights.transform.localPosition = Vector3.zero;
        targets.transform.parent = parent.transform;
        targets.transform.localPosition = Vector3.zero;

        for (int i = 0; i < num; i++)
        {
            // instantiate
            GameObject target = new GameObject("target_" + i.ToString("D3"));
            target.transform.parent = targets.transform;

            GameObject light = Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
            light.name = prefab.name + "_" + i.ToString("D3");
            light.transform.parent = beamLights.transform;
            var blc = light.GetComponent<BeamLightController>();
            blc.targetObject = target;
            blc.targetLight = targetLight;

            // position
            Vector3 pos = Vector3.zero;
            // circle
            //float t = Mathf.PI * 2 * i / num;
            //pos.x = Mathf.Cos(t) * radius;
            //pos.z = Mathf.Sin(t) * radius;
            // liner
            pos.x = interval * i;
            light.transform.localPosition = pos;
            target.transform.localPosition = pos;

            Undo.RegisterCreatedObjectUndo(light, "Create Buoys");

        }
    }
}