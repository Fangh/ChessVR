using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleInstantiator : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    public void Instantiate()
    {
        NetworkManager.Instance.Instantiate(prefab.name, Vector3.zero, Quaternion.identity, null);
    }
}
