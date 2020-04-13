using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabbable
{
    void StartGrab();
    void SyncUpGrab(Vector3 _pos);
    void StopGrab();
}