using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransformSync
{
    void SyncTransform(Vector3 _position, Quaternion _rotation, Vector3 _scale);
}