using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class GrabSphere : SyncMonoBehaviour, IGrabbable
{
    [Header("References")]
    [SerializeField] private Piece piece;
    [SerializeField] private PositionConstraint constraint;
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Settings")]
    [SerializeField] private Color grabbedColor;
    [SerializeField] private Color ungrabbedColor;

    private Transform pinchTransform;

    public void SetConstraint(Transform _model)
    {
        var source = new ConstraintSource
        {
            sourceTransform = _model,
            weight = 1
        };

        constraint.AddSource(source);
    }

    public void StartGrab()
    {
        NetworkManager.Instance.SendNetworkMessageToServer(new SNetworkMessage(EMessageType.Grab, GUID));
    }

    public void Grab()
    {
        constraint.enabled = false;
        piece.StopPhysics();
        meshRenderer.material.SetColor("_BaseColor", grabbedColor);
        Debug.Log($"{piece.gameObject.name} is grabbed", this);
    }

    public void SyncUpGrab(Vector3 _pos)
    {
        NetworkManager.Instance.SendNetworkMessageToServer(new SNetworkMessage(EMessageType.UpdateGrab, JsonUtility.ToJson(new SMessaveVector3(GUID, _pos))));
    }

    public void SyncDownGrab(Vector3 _pos)
    {
        transform.position = _pos;
        piece.FollowPinch(_pos);
    }

    public void StopGrab()
    {
        NetworkManager.Instance.SendNetworkMessageToServer(new SNetworkMessage(EMessageType.Ungrab, GUID));
    }

    public void UnGrab()
    {
        constraint.enabled = true;
        piece.ResumePhysics();
        meshRenderer.material.SetColor("_BaseColor", ungrabbedColor);
        Debug.Log($"{piece.gameObject.name} is ungrabbed", this);
    }

    public override void SyncTransform(Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        base.SyncTransform(_position, _rotation, _scale);
    }
}
