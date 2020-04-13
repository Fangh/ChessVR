using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSynchronizer : MonoBehaviour
{
    public float synchroTime = (1f / 30f);

    public static NetworkSynchronizer Instance;

    private Dictionary<string, SyncMonoBehaviour> synchronizedObjects = new Dictionary<string, SyncMonoBehaviour>();

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (SyncMonoBehaviour SMB in FindObjectsOfType<SyncMonoBehaviour>())
        {
            AddSynchronizeObject(SMB);
        }
    }

    public void AddSynchronizeObject(SyncMonoBehaviour _SMB)
    {
        synchronizedObjects.Add(_SMB.GUID, _SMB);
        Debug.Log($"Adding {_SMB.name} ({_SMB.GUID}) to synchronized objects");
    }

    //Send to server an object that has moved
    public void SyncUp(SyncMonoBehaviour SMB)
    {
        Debug.Log($"{SMB.name} has moved locally, synchro up");
        NetworkManager.Instance.SendNetworkMessageToServer(new SNetworkMessage(EMessageType.UpdateTransform, JsonUtility.ToJson(new SMessageUpdateTransform(SMB.GUID, SMB.transform.position, SMB.transform.rotation, SMB.transform.localScale))));

    }

    //Receive from server an object that has moved
    public void SyncDown(string _GUID, Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        Debug.Log($"{synchronizedObjects[_GUID].name} has moved, synchro down");
        synchronizedObjects[_GUID].SyncTransform(_position, _rotation, _scale);
    }

}
