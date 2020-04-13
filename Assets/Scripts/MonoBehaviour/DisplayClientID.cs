using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayClientID : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TMP;
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.OnClientConnected += Display;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Display()
    {
        TMP.text = $"Connected with ID ({NetworkManager.Instance.GetClientID()})";
    }
}
