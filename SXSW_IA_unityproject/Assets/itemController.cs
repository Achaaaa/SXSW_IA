using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemController : MonoBehaviour
{
    public bool isGrabbed;
    public GameObject hand; 
    //public Vector3 handPos;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isGrabbed){
            this.transform.position = hand.transform.position;
            this.transform.rotation = hand.transform.rotation;
        }
    }
}
