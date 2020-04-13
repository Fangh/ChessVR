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
    UpdateTransform,
    Instantiate,

    //only for Chess
    Grab,
    Ungrab,
    UpdateHand,
}


public struct SMessageInstantiate
{
    public string GUID;
    public string prefabName;
    public Vector3 position;
    public Quaternion rotation;
    public string parentName; //should be a unique name of a unique gameObject in the scene

    public SMessageInstantiate(string _prefabName, Vector3 _position, Quaternion _rotation, string _parentName)
    {
        GUID = null;
        prefabName = _prefabName;
        position = _position;
        rotation = _rotation;
        parentName = _parentName;
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

