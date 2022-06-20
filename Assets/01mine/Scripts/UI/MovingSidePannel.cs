using UnityEngine;
public class MovingSidePannel : MonoBehaviour
{
    [SerializeField]
    Vector2 hideMin, hideMax, showMin, showMax;
    [SerializeField]
    float speed, offsset;
    public bool toShow; 
    RectTransform mrect;
    void Start()
    {
        mrect = GetComponent<RectTransform>();
        mrect.anchorMax = hideMax;
        mrect.anchorMin = hideMin;
        toShow = false;
    }
    void Update()
    {
        Vector2 tarjectMin, tarjectMax;
        tarjectMin = toShow ? showMin : hideMin;
        tarjectMax = toShow ? showMax : hideMax;
        if (Vector2.Distance(mrect.anchorMin, tarjectMin) > offsset || Vector2.Distance(mrect.anchorMax, tarjectMax) > offsset)
        {
            
            if ((mrect.anchorMin - tarjectMin).magnitude <= (hideMin - showMin).magnitude / 10)
                mrect.anchorMin = tarjectMin;
            else
                mrect.anchorMin = Vector2.Lerp(mrect.anchorMin, tarjectMin, speed * Time.deltaTime);
            if ((mrect.anchorMax - tarjectMax).magnitude <= (hideMax - showMax).magnitude / 10)
                mrect.anchorMax = tarjectMax;
            else
                mrect.anchorMax = Vector2.Lerp(mrect.anchorMax, tarjectMax, speed * Time.deltaTime);
        } 
    }
}