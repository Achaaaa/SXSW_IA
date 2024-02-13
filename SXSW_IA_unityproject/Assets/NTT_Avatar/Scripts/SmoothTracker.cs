using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothTracker : MonoBehaviour
{
    [SerializeField] public Transform target;
    [SerializeField] public float positionSpeed = 40f;
    [SerializeField] public float rotationSpeed = .5f;
    [SerializeField] public bool shouldSmoothPosition = true;
    [SerializeField] public bool shouldSmoothRotation = true;

    [SerializeField] Transform rotationCenter;

    [SerializeField] public float bodyTwistRate = 1;
    [SerializeField] public Vector3 posOffset = new Vector3();

    float rotationDegree = 0;
    public Vector3 lastTargetPos = new Vector3();
    public Quaternion lastTargetRotation = new Quaternion();

    public Transform secondaryTarget = null;
    public float secondaryTargetWeight = 0;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = target.rotation;
        transform.position = target.position;
    }

    // Update is called once per frame
    void Update()
    {
        rotationDegree = MotionManager.shared.neck_angle * bodyTwistRate;

        if (shouldSmoothPosition) {
            UpdateSmoothPosition();
        } else {
            transform.position = target.position;
        }

        if (shouldSmoothRotation) {
            UpdateSmoothRotation();
        } else {
            transform.rotation = target.rotation;
        }
    }

    void UpdateSmoothPosition() {
        float delta = Time.deltaTime;
        float rate = 1 - Mathf.Pow(.5f, positionSpeed * delta);

        Vector3 pos = target.position;

        // DJ卓用
        if (secondaryTarget) {
            pos = Vector3.Lerp(pos, secondaryTarget.position, secondaryTargetWeight);
        }

        pos += rotationCenter.rotation * posOffset;
        
        pos = pos - rotationCenter.position;
        pos = Quaternion.AngleAxis(rotationDegree, Vector3.up) * pos;
        pos += rotationCenter.position;

        transform.position = Vector3.Lerp(transform.position, pos, rate);
        lastTargetPos = pos;
    }

    void UpdateSmoothRotation() {
        float delta = Time.deltaTime;
        float rate = 1 - Mathf.Pow(.5f, rotationSpeed * delta);

        Quaternion rot = target.rotation;

        // DJ卓用
        if (secondaryTarget) {
            rot = Quaternion.Slerp(rot, secondaryTarget.rotation, secondaryTargetWeight);
        }

        rot = Quaternion.AngleAxis(rotationDegree, Vector3.up) * rot;

        transform.rotation = Quaternion.Slerp(transform.rotation, rot, rate);
        lastTargetRotation = rot;
    }

    void CheckNaN() {
        Vector3 p = transform.position;
        if (float.IsNaN(p.x) || float.IsNaN(p.y) || float.IsNaN(p.z)) {
            transform.position = new Vector3();
        }

        Quaternion q = transform.rotation;
        if (float.IsNaN(q.x) || float.IsNaN(q.y) || float.IsNaN(q.z) || float.IsNaN(q.w)) {
            transform.rotation = Quaternion.identity;
        }
    }
}
