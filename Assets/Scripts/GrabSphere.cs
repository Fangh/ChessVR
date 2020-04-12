using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class GrabSphere : MonoBehaviour, IGrabbable
{
    [Header("References")]
    [SerializeField] private Piece piece;
    [SerializeField] private PositionConstraint constraint;

    [Header("Settings")]
    [SerializeField] private Color grabbedColor;
    [SerializeField] private Color ungrabbedColor;

    private MeshRenderer meshRenderer;
    private Transform pinchTransform;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void Grab()
    {
        constraint.enabled = false;
        piece.StopPhysics();
        meshRenderer.material.SetColor("_BaseColor", grabbedColor);
        Debug.Log($"{piece.gameObject.name} is grabbed", this);
    }

    public void UpdateGrab(Vector3 _pos)
    {
        transform.position = _pos;
        piece.FollowSphere(_pos);
    }

    public void UnGrab()
    {
        constraint.enabled = true;
        piece.ResumePhysics();
        meshRenderer.material.SetColor("_BaseColor", ungrabbedColor);
        Debug.Log($"{piece.gameObject.name} is ungrabbed", this);
    }

}
