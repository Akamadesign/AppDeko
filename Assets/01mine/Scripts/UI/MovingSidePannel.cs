using UnityEngine;
public class MovingSidePannel : MonoBehaviour
{
    [SerializeField]
    Vector2 hideMin, hideMax, showMin, showMax;
    [SerializeField]
    float speed;
    [SerializeField]
    bool show;
    RectTransform mrect;
    void Start()
    {
        mrect = GetComponent<RectTransform>();
        mrect.anchorMax = hideMax;
        mrect.anchorMin = hideMin;
        show = false;
    }
    void Update()
    {
        Vector2 tarjectMin, tarjectMax;
        tarjectMin = show ? showMin : hideMin;
        tarjectMax = show ? showMax : hideMax;
        if(mrect.anchorMin != tarjectMin || mrect.anchorMax != tarjectMax)
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
    public void ShowOrHidePanel(bool show)
    {
        this.show = show;
    }
}