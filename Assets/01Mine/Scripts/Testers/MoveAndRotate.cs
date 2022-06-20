using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MoveAndRotate : MonoBehaviour
{
    [SerializeField] Rigidbody selectedObject;
    [SerializeField] LayerMask floorLayerMask;
    Vector3 initialSelectedRotation, initialSelectedPosition, midFirstHit;
    Vector2 midStartPos, firstDirection;

    void Update()
    {
        if (Input.touchCount == 2)
        {
            print("Second Touch");
            if (Input.GetTouch(1).phase == TouchPhase.Began)
                StartMoveAndRotation();
            else if (Input.GetTouch(1).phase == TouchPhase.Ended)
                EndMovingAndRotatingSelected();
            else
                MovingAndRotatingSelected();
        }
    }
    void StartMoveAndRotation()
    {
        print("Starting To Rotate");
        firstDirection = Input.GetTouch(0).position - Input.GetTouch(1).position;
        initialSelectedRotation = selectedObject.transform.rotation.eulerAngles;
        midStartPos = Vector2.Lerp(Input.GetTouch(0).position, Input.GetTouch(1).position, 0.5f);
        Ray ray = Camera.main.ScreenPointToRay(midStartPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, floorLayerMask))
        {
            midFirstHit = hit.point;
            initialSelectedPosition = selectedObject.transform.position;
        }
    }

    void MovingAndRotatingSelected()
    {
        Vector2 newDirection = Input.GetTouch(0).position - Input.GetTouch(1).position;
        Vector2 midPos = Vector2.Lerp(Input.GetTouch(0).position, Input.GetTouch(1).position, 0.5f);
        float rotationAngle = Vector2.SignedAngle(firstDirection, newDirection);
        float newRotationAngle = initialSelectedRotation.y - rotationAngle;
        Vector3 newRotation = selectedObject.transform.rotation.eulerAngles;
        newRotation.y = newRotationAngle;
        Ray ray = Camera.main.ScreenPointToRay(midPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100, floorLayerMask))
        {
            if(hit.point != midFirstHit)
            {
                Vector3 movement = hit.point - midFirstHit;
                selectedObject.MovePosition(initialSelectedPosition + movement);
            }
        }
        selectedObject.MoveRotation(Quaternion.Euler(newRotation));
    }

    void EndMovingAndRotatingSelected()
    {
        firstDirection = Vector2.zero;
        midStartPos = Vector3.zero;
        midFirstHit = Vector3.zero;
        initialSelectedPosition = Vector3.zero;
        initialSelectedRotation = Vector3.zero;
    }

    //void StartMovingSelected()
    //{
    //    finguer0FirstPos = Input.GetTouch(0).position;
    //    Ray ray = Camera.main.ScreenPointToRay(finguer0FirstPos);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit, 100, floorLayerMask))
    //    {
    //        fingerFirstHit = hit.point;
    //        initialSelectedPosition = new Vector3(selectedObject.transform.position.x, selectedObject.transform.position.y, selectedObject.transform.position.z);
    //    }
    //}

    //void MovingSelected()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
    //    RaycastHit hit;
    //    if (Physics.Raycast(ray, out hit, 100, floorLayerMask))
    //    {
    //        if (hit.point != fingerFirstHit)
    //        {
    //            print("moving");
    //            Vector3 movement = hit.point - fingerFirstHit;
    //            selectedObject.transform.position = initialSelectedPosition + movement;
    //        }
    //    }
    //}

    //void EndMovingSelected()
    //{
    //    fingerFirstHit = Vector3.zero;
    //    initialSelectedPosition = Vector3.zero;
    //}

}
