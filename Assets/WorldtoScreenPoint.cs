using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldtoScreenPoint : MonoBehaviour
{
    public Camera mainCamera;
    public RectTransform textRectTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(transform.position);
        textRectTransform.position = screenPoint + new Vector3(-textRectTransform.rect.width / 2, textRectTransform.rect.height / 2, 0);
    }
}