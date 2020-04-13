using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FingerInteractable : MonoBehaviour
{
    public UnityEventEFinger OnFingerEnter;
    public UnityEventEFinger OnFingerExit;
}


[System.Serializable]
public class UnityEventEFinger : UnityEvent<EFinger> { }