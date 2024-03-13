using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBobbing : MonoBehaviour
{
    [SerializeField] private float bobbingSpeed = 4f;
    [SerializeField] private float bobbingHeight = 10f;
    float initialY;
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialY = rectTransform.anchoredPosition.y;
    }

    void Update()
    {
        rectTransform.anchoredPosition = new Vector3(rectTransform.anchoredPosition.x, initialY + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight, 0);
    }
}
