using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using TMPro;
using UnityEngine;

public class IPDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TMP;
    // Start is called before the first frame update
    void Start()
    {
        TMP.text = $"My IP : {Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault().MapToIPv4()}";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
