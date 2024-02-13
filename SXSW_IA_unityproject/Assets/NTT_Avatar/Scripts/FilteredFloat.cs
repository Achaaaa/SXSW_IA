using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FilteredFloat
{
    public float current = 0;
    public float target = 0;

    // filter
    public float mu;
    public float sigma;
    int index = 0;
    int length = 0;
    List<float> buffer = new List<float>();
    List<float> kernel = new List<float>();

    float targetFrameRate = 60;
    float prevUpdateTime = 0;

    public FilteredFloat(float filterWidth) {
        float mu = filterWidth * targetFrameRate;
        float sigma = mu / 3;
        SetupKernel(mu, sigma);
    }

    public void SetupKernel(float mu, float sigma) {
        this.mu = mu;
        this.sigma = sigma;

        length = (int)(mu * 2);

        buffer = Enumerable.Range(0, length).Select(i => 0f).ToList();
        kernel = Enumerable.Range(0, length).Select(x => 
            Mathf.Exp(-Mathf.Pow(x - mu, 2) / (2 * Mathf.Pow(sigma, 2)))
        ).ToList();

        float sum = kernel.Sum();
        kernel = kernel.Select(val => val / sum).ToList();
    }

    public void Update()
    {
        float time = Time.time;
        float deltaTime = time - prevUpdateTime;
        float targetDeltaTime = 1 / targetFrameRate;

        if (deltaTime < targetDeltaTime) {
            return;
        }

        prevUpdateTime = time;

        index += 1;
        if (index >= length) {
            index -= length;
        }

        buffer[index] = target;

        float total = 0;

        for (int i = 0; i < length; i++)
        {
            float k = kernel[i];
            int iv = (index - i);
            if (iv < 0) { iv += length; }
            float v = buffer[iv];

            total += v * k;
        }

        current = total;
    }
}
