using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.EventSystems;
public class AkaARManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("El objeto texto que mostrará mensajes en la pantalla")]
    [SerializeField] TMP_Text messagetxt;
    [Tooltip("Panel De Info de UI que se activaran segun las circunstacias")]
    public GameObject fittmentDetails;
    [Tooltip("Botones de UI que se activaran segun las circunstacias")]
    [SerializeField] GameObject reStart, cancelIt, addFitment, readytoPlace, fittmentInfo, deleteFittment;
    [Header("Behaviour Elements")]
    [Tooltip("Area minima a detectar para dar por hecho que hay un ''piso''")]
    [SerializeField] float minimunArea;
    [Tooltip("Objeto de piso que castea sombras")]
    [SerializeField] GameObject arFloor;
    [SerializeField] LayerMask selectable, floorLayerMask;
    ARPlaneManager arPlaneManager;
    ARRaycastManager aRRaycastManager;
    static List<ARRaycastHit> s_hits = new List<ARRaycastHit>();
    Vector2 pointer;
    GameObject objectToPlace, objectPlacing;
    Rigidbody arfloorRB;
    List<GameObject> objectsOnScene;
    Camera arCamera;

    public Rigidbody selectedObject;
    MyAppManager manager;
    Vector3 initialSelectedRotation, initialSelectedPosition, midFirstHit, arFloorStartPos;
    Vector2 midStartPos, firstDirection;

}
