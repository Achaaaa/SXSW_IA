using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorByWeight : MonoBehaviour
{
    float r;
    float b;
    // Update is called once per frame
    void Update()
    {
        r = MotionManager.shared.arm_R_point * 255.0f;
        b = 255.0f-r;
         GetComponent<Renderer>().material.color = new Color(r/255.0f, 0f, b/255.0f, 0.7f);
    }
}
