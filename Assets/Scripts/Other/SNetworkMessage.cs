using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SNetworkMessage
{
    public int clientID;
    public EMessageType type;
    public string JSON;

    public SNetworkMessage(EMessageType _type, string _json)
    {
        type = _type;
        JSON = _json;
        clientID = int.MinValue;
    }
}

public enum EMessageType
{
    Unknown,
    UpdateHand,
    UpdateTransform,
    Instantiate
}


public struct SMessageInstantiate
{
    public string GUID;
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;
    public Transform parent;

    public SMessageInstantiate(string _prefabName, Vector3 _position, Quaternion _rotation, Transform _parent)
    {
        GUID = null;
        prefabName = _prefabName;
        position = _position;
        rotation = _rotation;
        parent = _parent;
    }
}

public struct SMessageUpdateTransform
{
    public string GUID;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public SMessageUpdateTransform(string _GUID, Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        GUID = _GUID;
        position = _position;
        rotation = _rotation;
        scale = _scale;
    }
}