using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamLightController : MonoBehaviour
{
    public GameObject targetObject;
    public Light targetLight;
    public float speed = 2.0f;
    [Header("System")]
    public GameObject lightObject;
    public MeshRenderer lightShaft;

    Material lightMat;

    private void Start()
    {
        lightMat = lightShaft.material;
        lightMat.SetFloat("_LightIndex", Random.Range(0f, 1f));

        var mf = transform.Find("LightBase/LightIcos/LightShaft45").gameObject.GetComponent<MeshFilter>();
        var bounds = mf.mesh.bounds;
        bounds.size = Vector3.one * 1000f;
        mf.mesh.bounds = bounds;
    }

    void Update()
    {
        LookAtTarget();
        UpdateColor();
    }

    private void LookAtTarget()
    {
        var target = targetObject.transform;
        var origin = lightObject.transform;

        Vector3 direction = target.position - origin.position;
        Quaternion toRotation = Quaternion.FromToRotation(new Vector3(0, 0, 1), direction);
        origin.rotation = Quaternion.Lerp(origin.rotation, toRotation, speed * Time.deltaTime);
    }

    private void UpdateColor()
    {
        var color = targetLight.color;
        var intensity = targetLight.intensity;
        var lenght = targetLight.range;

        lightMat.SetColor("_Color", color);
        lightMat.SetFloat("_Intensity", intensity);
        lightMat.SetFloat("_ConeLength", lenght);
    }
}
