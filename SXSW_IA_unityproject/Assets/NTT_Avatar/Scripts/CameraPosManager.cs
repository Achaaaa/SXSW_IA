using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPosManager : MonoBehaviour
{
    Vector3 frontCameraPos = new Vector3(0,1.005f,2.5f);
    Quaternion frontCameraRot = new Quaternion(0,1,0,0);

    Vector3 backCameraPos = new Vector3(0,2,-2.5f);
    Quaternion backCameraRot = new Quaternion(0.173648164f,0,0,0.984807789f);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float speed = 0.99f;
        float rate = 1f - Mathf.Pow(1 - speed, Time.deltaTime);
        // float rate = 0.01f;

        bool isWalking = MotionManager.shared.legs_action == 2;

        Vector3 pos = transform.localPosition;
        Quaternion rot = transform.localRotation;

        Vector3 targetPos = isWalking ? backCameraPos : frontCameraPos;
        Quaternion targetRot = isWalking ? backCameraRot : frontCameraRot;

        pos = Vector3.Lerp(pos, targetPos, rate);
        rot = Quaternion.Slerp(rot, targetRot, rate);

        transform.localPosition = pos;
        transform.localRotation = rot;
    }
}
