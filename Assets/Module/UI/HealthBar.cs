using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public RectTransform progressBar;
    public TextMeshProUGUI valueDisplay;
    public HealthComponent healthComponent;

    private float _defaultWidth;

    // Start is called before the first frame update
    void Start()
    {
       

        _defaultWidth = progressBar.sizeDelta.x;
    }


    // Update is called once per frame
    void Update()
    {
        // TODO: Change this so we do no access directly the player but a data structure
        if (!healthComponent && GameManager.Instance.PlayerInstance)
        {
            healthComponent = GameManager.Instance.PlayerInstance.GetComponent<HealthComponent>();
        }

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
