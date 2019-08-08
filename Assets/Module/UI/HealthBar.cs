using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public HealthComponent healthComponent;
    public RectTransform progressBar;
    public TextMeshProUGUI valueDisplay;

    private float _defaultWidth;

    // Start is called before the first frame update
    void Start()
    {
        if (!healthComponent)
        {
            Debug.LogError("No health component were set on the healthBar.");
        }

        _defaultWidth = progressBar.sizeDelta.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (healthComponent)
        {
            float ratio = Mathf.Clamp(healthComponent.current / healthComponent.max, 0, 1);
            progressBar.sizeDelta = new Vector2(_defaultWidth * ratio, progressBar.sizeDelta.y);

            if (valueDisplay)
            {
                valueDisplay.SetText(healthComponent.current + "/" + healthComponent.max);
            }
        }
    }
}
