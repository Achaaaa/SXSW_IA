using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractManager : MonoBehaviour
{
    public itemController targetItem;
    public GameObject hitObj;
    public RaycastHit hit;
    private Vector3 rayDirection;
    private float rayLength = 1.0f;
    Ray ray;
    public GameObject rootObj;
    public bool intaractable;
    bool isRemovable;

    [SerializeField] private PlayerInput _playerInput;
    private InputAction _interact;

    void Awake()
    {
        _interact= _playerInput.actions["Interact"] ;
    }

    void Update()
    {
        rayDirection = this.transform.forward;
        ray = new Ray(rootObj.transform.position, rayDirection);
        Debug.DrawRay(ray.origin, ray.direction * rayLength, Color.green);

        if(_interact != null){
            if(_interact.IsPressed()){
                interactObject();
            }
        }
    }

    

    void interactObject()
    {
        // if(targetItem != null){
        //     targetItem.isGrabbed = false;
        //     targetItem = null;
        // }

        if (Physics.Raycast(ray.origin, ray.direction,out hit, rayLength))
        {
            Debug.Log(hit.collider.gameObject.name);
            targetItem = hit.collider.gameObject.GetComponent<itemController>();
            if (targetItem != null){
                Debug.Log(hit.collider.gameObject.name);
                targetItem.isGrabbed = true;
            }
        }
        else
        {
            targetItem = null;
            Debug.Log("noTarget");
        }
    }
}
