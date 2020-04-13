using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject model;

    [Header("Settings")]
    [SerializeField] private float followSmoothiness = 0.2f;
    [SerializeField] private Vector3 sphereOffset = new Vector3(0f, -0.7f, 0f);

    private Vector3 originalPos;
    private Quaternion originalRot;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public void StopPhysics()
    {
        model.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void ResumePhysics()
    {
        model.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void FollowPinch(Vector3 _pos)
    {
        Vector3 velocity = Vector3.zero;
        Vector3 targetPos = _pos + sphereOffset; 
        Vector3 meToTargetVector = _pos - model.transform.position;
        model.transform.rotation = Quaternion.FromToRotation(Vector3.up, meToTargetVector);
        model.transform.position = Vector3.SmoothDamp(model.transform.position, targetPos, ref velocity, followSmoothiness);
    }

    public void SetModel(GameObject _model)
    {
        model = Instantiate(_model, transform.position, Quaternion.identity);
        GetComponent<GrabSphere>().SetConstraint(model.transform);
    }
}
