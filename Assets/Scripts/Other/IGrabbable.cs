using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable
{
    void StartGrab();
    void UpdateGrab(Vector3 _pos);
    void StopGrab();
}