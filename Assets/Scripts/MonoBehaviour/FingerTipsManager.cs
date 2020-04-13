using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class FingerTipsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject fingerTipPrefab;
    [SerializeField] private GameObject pinchColliderPrefab;

    [Header("Settings")]
    [SerializeField] private float pinchSize = 0.01f;

    private Transform indexTip;
    private Transform thumbTip;
    private bool isPinching = true;

    private List<IGrabbable> grabbedObjects = new List<IGrabbable>();

    // Start is called before the first frame update
    void Start()
    {
        indexTip = FindChildByNameThatContains("IndexTip");
        thumbTip = FindChildByNameThatContains("ThumbTip");

        if (indexTip != null)
        {
            GameObject go = Instantiate(fingerTipPrefab, indexTip.position, indexTip.rotation, indexTip);
            FingerTip f = go.GetComponent<FingerTip>();
            f.finger = EFinger.Index;
            f.manager = this;
        }

        if (thumbTip != null)
        {
            GameObject go = Instantiate(fingerTipPrefab, thumbTip.position, thumbTip.rotation, thumbTip);
            FingerTip f = go.GetComponent<FingerTip>();
            f.finger = EFinger.Thumb;
            f.manager = this;
        }
    }

    private void Update()
    {
        if (isPinching)
        {
            Vector3 middle = thumbTip.position + (indexTip.position - thumbTip.position) * 0.5f;
            foreach (IGrabbable g in grabbedObjects)
            {
                g.UpdateGrab(middle);
            }
        }
    }

    public void StartPinch()
    {
        if (!isPinching)
        {
            Debug.Log($"Start pinching");
            isPinching = true;
            Vector3 thumbIndexVector = indexTip.position - thumbTip.position;
            Collider[] cols = Physics.OverlapSphere(thumbTip.position + thumbIndexVector * 0.5f, pinchSize);
            foreach (Collider c in cols)
            {
                IGrabbable grabbable = c.GetComponent<IGrabbable>();
                if (grabbable != null)
                {
                    grabbable.Grab();
                    grabbedObjects.Add(grabbable);
                }
            }
        }
    }

    public void StopPinch()
    {
        if (isPinching)
        {
            Debug.Log("Stop Pinching");
            isPinching = false;
            foreach (IGrabbable g in grabbedObjects)
            {
                g.UnGrab();
            }
            grabbedObjects = new List<IGrabbable>();
        }
    }

    private Transform FindChildByNameThatContains(string _string)
    {
        foreach (Transform child in transform.GetComponentsInChildren<Transform>())
        {
            if (child.name.Contains(_string))
                return child;
        }
        return null;
    }

}
