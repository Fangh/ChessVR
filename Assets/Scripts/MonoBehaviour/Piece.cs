using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class Piece : SyncMonoBehaviour, IGrabbable
{
    [Header("References")]
    [SerializeField] private GameObject model;
    [SerializeField] private PositionConstraint constraint;
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Settings")]
    [SerializeField] private Color grabbedColor;
    [SerializeField] private float followSmoothiness = 0.2f;
    [SerializeField] private Vector3 sphereOffset = new Vector3(0f, -0.7f, 0f);

    internal Color ungrabbedColor;
    private Transform pinchTransform;

    private void Start()
    {
        meshRenderer.material.SetColor("_BaseColor", ungrabbedColor);
    }

    private void StopPhysics()
    {
        model.GetComponent<Rigidbody>().isKinematic = true;
    }

    private void ResumePhysics()
    {
        model.GetComponent<Rigidbody>().isKinematic = false;
    }

    private void FollowPinch(Vector3 _pos)
    {
        Vector3 velocity = Vector3.zero;
        Vector3 targetPos = _pos + sphereOffset;
        Vector3 meToTargetVector = _pos - model.transform.position;
        model.transform.rotation = Quaternion.FromToRotation(Vector3.up, meToTargetVector);
        model.transform.position = Vector3.SmoothDamp(model.transform.position, targetPos, ref velocity, followSmoothiness);
    }

    public void AttachModel(GameObject _model)
    {
        model = _model;

        var source = new ConstraintSource
        {
            sourceTransform = model.transform,
            weight = 1
        };

        Debug.Log($"{name}: Attaching {_model.name}", this);

        constraint.AddSource(source);
    }

    public void StartGrab()
    {
        NetworkManager.Instance.SendNetworkMessageToServer(new SNetworkMessage(EMessageType.Grab, GUID));
    }

    public void Grab()
    {
        constraint.enabled = false;
        StopPhysics();
        meshRenderer.material.SetColor("_BaseColor", grabbedColor);
        Debug.Log($"{gameObject.name} is grabbed", this);
    }

    public void SyncUpGrab(Vector3 _pos)
    {
        NetworkManager.Instance.SendNetworkMessageToServer(new SNetworkMessage(EMessageType.UpdateGrab, JsonUtility.ToJson(new SMessageVector3(GUID, _pos))));
    }

    public void SyncDownGrab(Vector3 _pos)
    {
        transform.position = _pos;
        FollowPinch(_pos);
    }

    public void StopGrab()
    {
        NetworkManager.Instance.SendNetworkMessageToServer(new SNetworkMessage(EMessageType.Ungrab, GUID));
    }

    public void UnGrab()
    {
        constraint.enabled = true;
        ResumePhysics();
        meshRenderer.material.SetColor("_BaseColor", ungrabbedColor);
        Debug.Log($"{gameObject.name} is ungrabbed", this);
    }

    public override void SyncTransform(Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        base.SyncTransform(_position, _rotation, _scale);
    }
}
