using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rigidbody;

    [Header("Settings")]
    [SerializeField] private float followSmoothiness = 0.2f;
    [SerializeField] private Vector3 sphereOffset = new Vector3(0f, -0.7f, 0f);

    private Vector3 originalPos;
    private Quaternion originalRot;


    // Start is called before the first frame update
    void Start()
    {
        //originalPos = rigidbody.transform.localPosition;
        //originalRot = rigidbody.transform.localRotation;
    }

    // Update is called once per frame
    public void StopPhysics()
    {
        rigidbody.isKinematic = true;
        //transform.localPosition = originalPos;
        //transform.localRotation = originalRot;
    }

    public void ResumePhysics()
    {
        //originalPos = rigidbody.transform.localPosition;
        //originalRot = rigidbody.transform.localRotation;
        rigidbody.isKinematic = false;
    }

    public void FollowSphere(Vector3 _pos)
    {
        Vector3 velocity = Vector3.zero;
        Vector3 targetPos = _pos + sphereOffset; 
        Vector3 meToTargetVector = _pos - rigidbody.transform.position;
        rigidbody.transform.rotation = Quaternion.FromToRotation(Vector3.up, meToTargetVector);
        rigidbody.transform.position = Vector3.SmoothDamp(rigidbody.transform.position, targetPos, ref velocity, followSmoothiness);
    }

    public void SetModel(GameObject _model)
    {
        Instantiate(_model, rigidbody.transform);
    }
}
