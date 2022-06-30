using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingAnimation : MonoBehaviour
{
    List<RectTransform> dots;
    [SerializeField] Vector2 min, mid, max;
    [SerializeField] Color minC, maxC;
    [SerializeField] Vector3 minS, maxS;
    [SerializeField] float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 tarject1, tarject2, tarject3;

        for (int i = 0; i < dots.Count; i++)
        {

        }
    }
}
