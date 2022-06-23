using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody), typeof(BoxCollider), typeof(Outline))]
public class Selectable : MonoBehaviour
{
    Rigidbody rb;
    Outline outline;
    public bool selected;
    public string productID, variantID;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Selectthis()
    {

    }
}
