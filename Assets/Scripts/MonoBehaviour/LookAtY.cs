using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtY : MonoBehaviour
{
    public Transform sphere;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 meToSphereVector = sphere.position - transform.position;
        transform.rotation = Quaternion.FromToRotation(Vector3.up, meToSphereVector);
    }
}
