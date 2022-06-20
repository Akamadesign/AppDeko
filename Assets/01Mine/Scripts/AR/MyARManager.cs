using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.EventSystems;
public class MyARManager : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("El objeto texto que mostrará mensajes en la pantalla")]
    [SerializeField] TMP_Text messagetxt;
    [Tooltip("Botones de UI que se activaran segun las circunstacias")]
    [SerializeField] GameObject reStart, cancelIt, addFitment, readytoPlace;
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
    Rigidbody selectedObject;
    List<GameObject> objectsOnScene;
    Camera arCamera;
    MyAppManager manager;
    Vector3 initialSelectedRotation, initialSelectedPosition, midFirstHit;
    Vector2 midStartPos, firstDirection;
    private void Awake()
    {
        manager = FindObjectOfType<MyAppManager>();
        if (manager != null && manager.fittmentToLoad != null && manager.fittmentToLoad != null)
            AddThisFittment(AkaPrefabs.LoadedPrefabs[manager.fittmentToLoad]);
    }
    void Start()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        aRRaycastManager = GetComponent<ARRaycastManager>();
        objectsOnScene = new List<GameObject>();
        pointer = new Vector2(Screen.width / 2, Screen.height / 2);
        arCamera = Camera.main;
        arFloor.SetActive(false);
    }
    void Update()
    {
        if (!arFloor.activeInHierarchy)//the ar floor is not active in scene
        {
            reStart.SetActive(false);
            cancelIt.SetActive(false);
            addFitment.SetActive(false);
            readytoPlace.SetActive(false);
            SearchForFloor();
        }
        else // the arFloor is active in the scene
        {
            if (objectToPlace) //there is an object pending to be placed
            {
                reStart.SetActive(false);
                cancelIt.SetActive(true);
                addFitment.SetActive(false);
                readytoPlace.SetActive(true);
                PreplaceObject(); //place the object and select it
            }
            else //there is not an object to be placed 
            {
                if(selectedObject != null)//there is an object selected so we can move it and rotate it
                {
                    if (Input.touchCount == 2) //we will only move and rotate the selected object if we are using two finguers
                    {
                        print("Second Touch");
                        if (Input.GetTouch(1).phase == TouchPhase.Began)
                            StartMoveAndRotation();
                        else if (Input.GetTouch(1).phase == TouchPhase.Ended)
                            EndMovingAndRotatingSelected();
                        else
                            MovingAndRotatingSelected();
                    }
                }else
                {
                    LookingForSelection();
                }
                reStart.SetActive(true);
                cancelIt.SetActive(false);
                addFitment.SetActive(true);
                readytoPlace.SetActive(false);
            }
        }
    }

    void LookingForSelection()
    {
        if (Input.touchCount == 1 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100, selectable))
            {
                selectedObject = hit.collider.GetComponent<Rigidbody>();
            }
        }
    }
    /// <summary>
    /// Destroy all previous calculations on world detection and start them again
    /// </summary>
    public void RestartFloor()
    {
        foreach (GameObject g in objectsOnScene)
        {
            Destroy(g);
        }
        objectsOnScene.Clear();
        arFloor.SetActive(false);
        FindObjectOfType<ARSession>().Reset();
        arPlaneManager.enabled = true;
    }
    /// <summary>
    /// add a fittmet piece to the scene so we can place it
    /// </summary>
    /// <param name="prefab">prefab of the incoming GameObject</param>
    public void AddThisFittment(GameObject prefab)
    {
        objectToPlace = prefab;
    }
    /// <summary>
    /// Finds a suitable space to work with the minimun area required,
    /// when founded, place the arFloor on its place and turn off arcore plane detections
    /// </summary>
    void SearchForFloor()
    {
        messagetxt.text = "Mueve tu Dispositivo para escanear el suelo";
        if (aRRaycastManager.Raycast(pointer, s_hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
        {
            foreach (ARRaycastHit hit in s_hits)
            {
                ARPlane thisplane = (ARPlane)hit.trackable;
                float planeArea = thisplane.size.x * thisplane.size.y;
                if (planeArea > minimunArea)
                {
                    arFloor.transform.SetPositionAndRotation(hit.pose.position, hit.pose.rotation);
                    arFloor.SetActive(true);
                }
            }
        }
        if (arFloor.activeInHierarchy)
        {
            foreach (ARPlane plane in arPlaneManager.trackables)
            {
                Destroy(plane.gameObject);
            }
            arPlaneManager.enabled = false;
        }
    }
    /// <summary>
    /// instantiate the objectToPlace in to the scene so we can move it arround just pointing on some direction
    /// </summary>
    void PreplaceObject()
    {
        if (objectsOnScene.Count == 0)
            messagetxt.text = "Trata de ubicar tu mueble";
        Ray ray = arCamera.ScreenPointToRay(pointer);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, floorLayerMask))
        {
            if (hit.collider.gameObject == arFloor)
            {
                if (objectPlacing == null || !objectPlacing.gameObject.activeInHierarchy) 
                {
                    objectPlacing = Instantiate(objectToPlace, hit.point, hit.transform.rotation);
                    objectPlacing.GetComponent<Outline>().enabled = true;
                }
                else
                {
                    objectPlacing.transform.position = hit.point;
                    objectPlacing.transform.rotation = hit.transform.rotation;
                }
            }
        }
    }
    /// <summary>
    /// place the object to a certain space on scene and pre-select it so we can refine the position and rotation of the object
    /// </summary>
    public void PlaceFittment()
    {
        objectToPlace = null;
        objectsOnScene.Add(objectPlacing);
        selectedObject = objectPlacing.GetComponent<Rigidbody>();
        objectPlacing = null;
    }
    /// <summary>
        /// get the inicial values for movement and rotation to reference later
        /// </summary>
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
    /// <summary>
    /// get current petition to move and rotate the selected object
    /// </summary>
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
            if (hit.point != midFirstHit)
            {
                Vector3 movement = hit.point - midFirstHit;
                selectedObject.MovePosition(initialSelectedPosition + movement);
            }
        }
        selectedObject.MoveRotation(Quaternion.Euler(newRotation));
    }
    /// <summary>
    /// reset the inisializatiion variables
    /// </summary>
    void EndMovingAndRotatingSelected()
    {
        firstDirection = Vector2.zero;
        midStartPos = Vector3.zero;
        midFirstHit = Vector3.zero;
        initialSelectedPosition = Vector3.zero;
        initialSelectedRotation = Vector3.zero;
    }
}
