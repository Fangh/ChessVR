using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkSynchronizer : MonoBehaviour
{
    public float synchroTime = (1f / 30f);

    [SerializeField] private HandSynchronizer playerHands;
    [SerializeField] private HandSynchronizer otherPlayerHands;

    public static NetworkSynchronizer Instance;

    private Dictionary<string, SyncMonoBehaviour> synchronizedObjects = new Dictionary<string, SyncMonoBehaviour>();

    private float currentSynchroTime = 0;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentSynchroTime = synchroTime;

        foreach (SyncMonoBehaviour SMB in FindObjectsOfType<SyncMonoBehaviour>())
        {
            AddSynchronizeObject(SMB);
        }
    }

    private void Update()
    {
        if (!NetworkManager.Instance.isConnected)
            return;

        if(currentSynchroTime <= 0)
        {
            currentSynchroTime = synchroTime;
            playerHands.SyncHandUp();
        }
        else
        {
            currentSynchroTime -= Time.deltaTime;
        }
    }

    public void AddSynchronizeObject(SyncMonoBehaviour _SMB)
    {
        synchronizedObjects.Add(_SMB.GUID, _SMB);
        Debug.Log($"Adding {_SMB.name} ({_SMB.GUID}) to synchronized objects", _SMB);
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

    public SyncMonoBehaviour GetSMBByGUID(string _GUID)
    {
        return synchronizedObjects[_GUID];
    }


    public void SyncHandsDown(SMessageHand _message)
    {
        otherPlayerHands.SyncHandDown(_message);
    }

}
