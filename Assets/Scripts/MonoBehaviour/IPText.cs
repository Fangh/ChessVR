using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IPText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI TMP;
    [SerializeField] private Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        TMP.text = "";
        NetworkManager.OnClientConnected += DisableMe;
    }

    private void DisableMe()
    {
        canvas.gameObject.SetActive(false);
    }

    public void Addstring(string str)
    {
        TMP.text += str;
    }
    public void BackSpace()
    {
        TMP.text = TMP.text.Remove(TMP.text.Length - 1);
    }

}
