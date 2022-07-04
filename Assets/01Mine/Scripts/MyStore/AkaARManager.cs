using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;
using UnityEngine.EventSystems;
public class AkaARManager : MonoBehaviour
{
    public enum State {FloorLess, EmptyScene, WithFurniture, Selected, Moving, Details}
    [Header("State")]
    public State estado;
    [Header("UI Elements")]
    [Tooltip("El objeto texto que mostrará mensajes en la pantalla")]
    [SerializeField] TMP_Text messagetxt;
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
    GameObject objectToPlace, objectPlacing;
    Rigidbody arfloorRB;
    List<GameObject> objectsOnScene;
    Camera arCamera;
    public Rigidbody selectedObject;
    Vector3 initialSelectedRotation, initialSelectedPosition, midFirstHit, arFloorStartPos;
    Vector2 midStartPos, firstDirection, midScreenPoint;
    AkaStore akaStore;
    private void OnEnable()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        aRRaycastManager = GetComponent<ARRaycastManager>();
        objectsOnScene = new List<GameObject>();
        midScreenPoint = new Vector2(Screen.width / 2, Screen.height / 2);
        arCamera = Camera.main;
        arFloor.SetActive(false);
        akaStore = FindObjectOfType<AkaStore>();
    }
    void Update()
    {
        if (!arFloor.activeInHierarchy && akaStore.raPannel.gameObject.activeInHierarchy)
            SearchForFloor();
        else
        {
            if (objectToPlace != null)
                PreplaceObject();
            else
            {
                LookingForSelection();
                if (selectedObject != null)
                {
                    if (Input.touchCount == 2)
                    {
                        if (Input.GetTouch(1).phase == TouchPhase.Began)
                            StartMoveAndRotation();
                        else if (Input.GetTouch(1).phase == TouchPhase.Ended)
                            EndMovingAndRotatingSelected();
                        else
                            MovingAndRotatingSelected();
                    }
                }
            }
        }
        reStart.SetActive       (arFloor.activeInHierarchy && objectToPlace == null && selectedObject == null);
        addFitment.SetActive    (arFloor.activeInHierarchy && objectToPlace == null && selectedObject == null);
        readytoPlace.SetActive  (arFloor.activeInHierarchy && objectToPlace != null);
        cancelIt.SetActive      (arFloor.activeInHierarchy && objectToPlace != null);
        fittmentInfo.SetActive  (arFloor.activeInHierarchy && objectToPlace == null && selectedObject != null);
        deleteFittment.SetActive(arFloor.activeInHierarchy && objectToPlace == null && selectedObject != null);
    }
    void SearchForFloor()
    {
        messagetxt.text = "Mueve tu Dispositivo para escanear el suelo";
        if (aRRaycastManager.Raycast(midScreenPoint, s_hits, UnityEngine.XR.ARSubsystems.TrackableType.Planes))
        {
            foreach (ARRaycastHit hit in s_hits)
            {
                ARPlane thisplane = (ARPlane)hit.trackable;
                float planeArea = thisplane.size.x * thisplane.size.y;
                if (planeArea > minimunArea)
                {
                    GameObject arOrigin = FindObjectOfType<ARSessionOrigin>().gameObject;
                    arOrigin.transform.SetPositionAndRotation(hit.pose.position, hit.pose.rotation);
                    arFloor.transform.SetPositionAndRotation(hit.pose.position, hit.pose.rotation);
                    arFloor.SetActive(true);
                    arFloorStartPos = arFloor.transform.position;
                    arfloorRB = arFloor.GetComponent<Rigidbody>();
                }
            }
        }
#if UNITY_EDITOR
        arFloor.SetActive(true);
        arfloorRB = arFloor.GetComponent<Rigidbody>();
        arFloorStartPos = arFloor.transform.position;
#endif
        if (arFloor.activeInHierarchy)
        {
            foreach (ARPlane plane in arPlaneManager.trackables)
            {
                Destroy(plane.gameObject);
            }
            arPlaneManager.enabled = false;
        }
    }
    void LookingForSelection()
    {
        if (selectedObject == null)
            messagetxt.text = "Selecciona con un toque";
        else
            messagetxt.text = "Toca otra cosa para desseleccionar";
        if (Input.touchCount == 1
            && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
            && Input.GetTouch(0).phase != TouchPhase.Ended
            && Input.GetTouch(0).phase != TouchPhase.Moved)
        {
            akaStore = FindObjectOfType<AkaStore>();
            akaStore.GetComponent<MovingSidePannel>().HideOrShowPanel(false);
            akaStore.detailsManager.gameObject.SetActive(true);
        
            Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;
            ClearSelection();
            if (Physics.Raycast(ray, out hit, 100, selectable))
            {
                selectedObject = hit.collider.GetComponent<Rigidbody>();
                selectedObject.GetComponent<Outline>().enabled = true;
                selectedObject.GetComponent<Outline>().OutlineWidth = 10;
                selectedObject.isKinematic = false;
            }
        }
    }
    
    void PreplaceObject()
    {
        if (objectsOnScene.Count == 0)
            messagetxt.text = "Trata de ubicar tu mueble";
        else
            messagetxt.text = "";
        Ray ray = arCamera.ScreenPointToRay(midScreenPoint);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, floorLayerMask))
        {
            if (objectPlacing == null || !objectPlacing.gameObject.activeInHierarchy)
            {
                objectPlacing = Instantiate(objectToPlace, hit.point, hit.transform.rotation, arFloor.transform);
                objectPlacing.GetComponent<Outline>().enabled = true;
                objectPlacing.GetComponent<Outline>().OutlineWidth = 10;
            }
            else
            {
                objectPlacing.transform.position = hit.point;
                objectPlacing.transform.rotation = hit.transform.rotation;
            }
        }
    }
    void StartMoveAndRotation()
    {
        firstDirection = Input.GetTouch(0).position - Input.GetTouch(1).position;
        initialSelectedRotation = selectedObject.transform.localEulerAngles;
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
        Vector3 newRotation = selectedObject.transform.localEulerAngles;
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
        selectedObject.transform.localRotation = Quaternion.Euler(newRotation);
    }
    void EndMovingAndRotatingSelected()
    {
        firstDirection = Vector2.zero;
        midStartPos = Vector3.zero;
        midFirstHit = Vector3.zero;
        initialSelectedPosition = Vector3.zero;
        initialSelectedRotation = Vector3.zero;
    }
    public void AddThisFittment(GameObject prefab)
    {
        objectToPlace = prefab;
    }
    public void PlaceFittment()
    {
        objectToPlace = null;
        objectsOnScene.Add(objectPlacing);
        selectedObject = objectPlacing.GetComponent<Rigidbody>();
        selectedObject.GetComponent<Outline>().enabled = true;
        selectedObject.GetComponent<Outline>().OutlineWidth = 10;
        selectedObject.isKinematic = false;
        objectPlacing = null;
    }
    public void CancelPlacing()
    {
        objectToPlace = null;
        objectPlacing = null;
    }
    public void GetFittmentInfo()
    {
        print("trying to dyplay info");
        AkaSelectable ss = selectedObject.GetComponent<AkaSelectable>();
        Shopify.Unity.Product pr = null;
        Shopify.Unity.ProductVariant vr = null;
        AkaProductsList.productsOnStore.TryGetValue(ss.productID, out pr);
        List<Shopify.Unity.ProductVariant> variantes = (List<Shopify.Unity.ProductVariant>)pr.variants();
        foreach (var item in variantes)
            if (item.id() == ss.variantID)
                vr = item;
        akaStore = FindObjectOfType<AkaStore>();
        akaStore.GetComponent<MovingSidePannel>().HideOrShowPanel(true);
        akaStore.detailsManager.FillWithDetails(pr, vr);
    }
    public void DeleteSelectedItem()
    {
        print("trying to delete fittment");
        Destroy(selectedObject.gameObject);
        selectedObject = null;
    }
    public void ReplacePrefab(GameObject newPrefab)
    {
        Pose keppPose = new Pose(selectedObject.transform.position, selectedObject.transform.rotation);
        DeleteSelectedItem();
        selectedObject = Instantiate(newPrefab, keppPose.position, keppPose.rotation, arFloor.transform).GetComponent<Rigidbody>();
        selectedObject.GetComponent<Outline>().enabled = true;
        selectedObject.GetComponent<Outline>().OutlineWidth = 10;
    }
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
    public void SetFloorHeigth(float newHeigth)
    {
        if (arfloorRB == null)
            arfloorRB = arFloor.GetComponent<Rigidbody>();
        float newheigth = Mathf.Lerp(-1, 1, newHeigth);
        Vector3 newPos = arFloorStartPos + (arFloor.transform.up * newheigth);
        arfloorRB.MovePosition(newPos);
    }
    public void ClearSelection()
    {
        if (selectedObject != null)
        {
            selectedObject.isKinematic = true;
            selectedObject.GetComponent<Outline>().enabled = false;
            selectedObject = null;
        }
    }
}