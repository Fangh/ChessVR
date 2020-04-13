using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TouchableUIButton : FingerInteractable
{
    private void Reset()
    {
        var Rect = GetComponent<RectTransform>();
        GetComponent<BoxCollider>().size = new Vector3(Rect.sizeDelta.x, Rect.sizeDelta.y, 1);
    }

}
