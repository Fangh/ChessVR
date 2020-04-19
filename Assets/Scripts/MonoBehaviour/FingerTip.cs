using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FingerTip : MonoBehaviour
{
    internal EFinger finger;
    internal FingerTipsManager manager;

    private bool isPushed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<FingerInteractable>() && !isPushed)
        {
            other.GetComponent<FingerInteractable>().OnFingerEnter?.Invoke(finger);
            isPushed = true;
        }
        else if (other.GetComponent<FingerTip>())
        {
            if (finger == EFinger.Thumb && other.GetComponent<FingerTip>().finger == EFinger.Index)
            {
                Debug.Log("thumb & index are touching");
                manager.StartPinch();
            }
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<FingerInteractable>() && isPushed)
        {
            other.GetComponent<FingerInteractable>().OnFingerExit?.Invoke(finger);
            isPushed = false;
        }
        else if (other.GetComponent<FingerTip>())
        {
            if (finger == EFinger.Thumb && other.GetComponent<FingerTip>().finger == EFinger.Index)
            {
                Debug.Log("thumb & index are not touching anymore");
                manager.StopPinch();
            }
        }
    }
}

[System.Serializable]
public enum EFinger
{
    Thumb,
    Index,
}