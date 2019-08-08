using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        GetComponent<Renderer>().UpdateGIMaterials();
    }
}
