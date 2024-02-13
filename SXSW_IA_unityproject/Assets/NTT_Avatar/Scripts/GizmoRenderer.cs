using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GizmoRenderer : MonoBehaviour
{
    [SerializeField] Color color = new (1, 0, 0);
    [SerializeField] float radius = 0.1f;

    SmoothTracker tracker = null;

    // Start is called before the first frame update
    void Start()
    {
        tracker = GetComponent<SmoothTracker>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos() {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        if (tracker) {
            position = tracker.lastTargetPos;
            rotation = tracker.lastTargetRotation;
        }
        

        var cache = Gizmos.matrix;
        Gizmos.matrix = Matrix4x4.TRS(position, rotation, transform.lossyScale);

        Gizmos.color = color;
        Gizmos.DrawWireSphere(Vector3.zero, radius);
        // Gizmos.DrawWireCube(Vector3.zero, Vector3.one * 1.1f);
        Gizmos.matrix = cache;

    }
}
