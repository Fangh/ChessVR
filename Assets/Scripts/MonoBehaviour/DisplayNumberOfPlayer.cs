using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayNumberOfPlayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TMP;
    // Start is called before the first frame update
    void Start()
    {
        NetworkManager.OnServerStarted += Display;
        NetworkManager.OnNewClient += UpdateDiplay;
        gameObject.SetActive(false);
    }

    private void UpdateDiplay(int obj)
    {
        Display();
    }

    private void Display()
    {
        gameObject.SetActive(true);
        TMP.text = $"{NetworkManager.Instance.GetNumberOfConnectedClients()} clients connected.";
    }

}
