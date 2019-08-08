using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class FixCanvasRotation : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Quaternion _fixedChildRotation;

    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _fixedChildRotation = _rectTransform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _rectTransform.rotation = _fixedChildRotation;
    }
}
