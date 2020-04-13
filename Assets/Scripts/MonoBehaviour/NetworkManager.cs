using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string serverIP = "127.0.0.1";

    private Telepathy.Server server;
    private Telepathy.Client client;

    private List<int> connectedClients = new List<int>();
    private List<SyncMonoBehaviour> syncObjects = new List<SyncMonoBehaviour>();

    public static NetworkManager Instance;

    void Awake()
    {
        Instance = this;

        // update even if window isn't focused, otherwise we don't receive.
        Application.runInBackground = true;

        // use Debug.Log functions for Telepathy so we can see it in the console
        Telepathy.Logger.Log = Debug.Log;
        Telepathy.Logger.LogWarning = Debug.LogWarning;
        Telepathy.Logger.LogError = Debug.LogError;
    }

    void OnApplicationQuit()
    {
        // the client/server threads won't receive the OnQuit info if we are
        // running them in the Editor. they would only quit when we press Play
        // again later. this is fine, but let's shut them down here for consistency
        client.Disconnect();
        server.Stop();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (server != null && server.Active)
            ReceiveServerMessages();

        if (client != null && client.Connected)
            ReceiveClientMessages();
    }

    private void ReceiveServerMessages()
    {
        // show all new messages
        while (server.GetNextMessage(out Telepathy.Message msg))
        {
            switch (msg.eventType)
            {
                case Telepathy.EventType.Connected:
                    Debug.Log($"client {msg.connectionId} is Connected");
                    connectedClients.Add(msg.connectionId);
                    break;
                case Telepathy.EventType.Disconnected:
                    Debug.Log(msg.connectionId + " Disconnected");
                    connectedClients.Remove(msg.connectionId);
                    break;
                case Telepathy.EventType.Data:
                    SNetworkMessage netMsg = JsonUtility.FromJson<SNetworkMessage>(Encoding.UTF8.GetString(msg.data));
                    netMsg.clientID = msg.connectionId;
                    Debug.Log($"client({msg.connectionId} has send a {netMsg.type} message.");
                    SortServerMessages(netMsg);
                    break;
            }
        }
    }

    private void SortServerMessages(SNetworkMessage _message)
    {
        if (_message.type == EMessageType.Unknown)
        {
            Debug.LogError("Server received an Unknow message");
            return;
        }

        if (_message.type == EMessageType.Instantiate)
        {
            SMessageInstantiate msg = JsonUtility.FromJson<SMessageInstantiate>(_message.JSON);
            msg.GUID = Guid.NewGuid().ToString();
            SendNetworkMessageToAllClients(new SNetworkMessage(EMessageType.Instantiate, JsonUtility.ToJson(msg)));
        }
    }

    private void ReceiveClientMessages()
    {
        // show all new messages
        while (client.GetNextMessage(out Telepathy.Message msg))
        {
            switch (msg.eventType)
            {
                case Telepathy.EventType.Connected:
                    Debug.Log(msg.connectionId + " Connected");
                    break;
                case Telepathy.EventType.Disconnected:
                    Debug.Log(msg.connectionId + " Disconnected");
                    break;
                case Telepathy.EventType.Data:
                    SNetworkMessage netMsg = JsonUtility.FromJson<SNetworkMessage>(Encoding.UTF8.GetString(msg.data));
                    Debug.Log($"server has send a {netMsg.type} message.");
                    SortClientMessages(netMsg);
                    break;
            }
        }
    }

    private void SortClientMessages(SNetworkMessage _message)
    {
        if (_message.type == EMessageType.Unknown)
        {
            Debug.LogError("Server sent an Unknow message");
            return;
        }

        if (_message.type == EMessageType.Instantiate)
        {
            SMessageInstantiate msg = JsonUtility.FromJson<SMessageInstantiate>(_message.JSON);
            InstantiateSyncObject(msg.prefabName, msg.GUID, msg.position, msg.rotation, msg.parent);
            return;
        }

        if(_message.type == EMessageType.UpdateTransform)
        {
            SMessageUpdateTransform msg = JsonUtility.FromJson<SMessageUpdateTransform>(_message.JSON);
            NetworkSynchronizer.Instance.SyncDown(msg.GUID, msg.position, msg.rotation, msg.scale);
            return;
        }
    }

    private void SendNetworkMessageToAllClients(SNetworkMessage _message)
    {
        foreach (int clientId in connectedClients)
        {
            SendNetworkMessageToClient(clientId, _message);
        }
    }

    private void SendNetworkMessageToClient(int _clientID, SNetworkMessage _message)
    {
        server.Send(_clientID, Encoding.UTF8.GetBytes(JsonUtility.ToJson(_message)));
    }

    public void SendNetworkMessageToServer(SNetworkMessage _message)
    {
        client.Send(Encoding.UTF8.GetBytes(JsonUtility.ToJson(_message)));
    }

    /// <summary>
    /// Called by the server on the clients to instantiate a Synchronized GameObject
    /// </summary>
    private void InstantiateSyncObject(string _prefabName, string _GUID, Vector3 _position, Quaternion _rotation, Transform _parent = null)
    {
        if (string.IsNullOrEmpty(_GUID))
        {
            Debug.LogError("You tried to instantiate a Game Object without a GUID");
            return;
        }

        GameObject instance = Instantiate(Resources.Load<GameObject>(_prefabName), _position, _rotation, _parent) as GameObject;
        SyncMonoBehaviour synchro = instance.GetComponent<SyncMonoBehaviour>();

        if (synchro == null)
        {
            Debug.LogError("You tried to instantiate a prefab which does not have a SyncMonoBehaviour");
            Destroy(instance);
            return;
        }

        synchro.GUID = _GUID;
    }

    /// <summary>
    /// Called by a client on the client to ask the server to instantiate a GameObject for all clients
    /// </summary>
    /// <param name="_prefabName">The name of the prefab. It should be the exact name of the prefab file that is in the Resource folder.</param>
    /// <param name="_position">The world pos of the instantiated GameObject</param>
    /// <param name="_rotation">The world rotation of the instantiated GameObject</param>
    /// <param name="_parent">The parent of the instantiated GameObject, can be null</param>
    public void Instantiate(string _prefabName, Vector3 _position, Quaternion _rotation, Transform _parent)
    {
        SendNetworkMessageToServer(new SNetworkMessage(EMessageType.Instantiate, JsonUtility.ToJson(new SMessageInstantiate(_prefabName, _position, _rotation, _parent))));
    }

    public void StartServer()
    {
        server = new Telepathy.Server();
        server.Start(4242);
    }

    public void StartClient()
    {
        client = new Telepathy.Client();
        client.Connect(serverIP, 4242);
    }
}
