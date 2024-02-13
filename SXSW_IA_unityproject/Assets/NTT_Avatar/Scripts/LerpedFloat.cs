using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpedFloat
{
    public float current = 0;
    public float target = 0;
    public float speed = 0.5f;

    public LerpedFloat() { }

    public LerpedFloat(float value) {
        this.current = value;
        this.target = value;
    }

    public LerpedFloat(float value, float speed) {
        this.current = value;
        this.target = value;
        this.speed = speed;
    }

    public void Update() {
        if (float.IsNaN(current)) { current = 0f; }
        current = Mathf.Lerp(current, target, speed);
    }
}
