﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string serverIP = "127.0.0.1";
    [SerializeField] private int port = 424242;

    private Telepathy.Server server;
    private Telepathy.Client client;

    private List<int> connectedClients = new List<int>();
    private List<SyncMonoBehaviour> syncObjects = new List<SyncMonoBehaviour>();

    public static NetworkManager Instance;

    internal bool isConnected = false;
    internal bool isServer = false;
    public static event Action<GameObject> OnInstantiate;
    public static event Action OnServerStarted;
    public static event Action<int> OnNewClient;
    public static event Action OnClientConnected;

    private int clientID = int.MinValue;

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
        if (client != null)
            client.Disconnect();
        if (server != null)
            server.Stop();
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
                    SendNetworkMessageToClient(msg.connectionId, new SNetworkMessage(EMessageType.ConnectionSuccessful, msg.connectionId.ToString()));
                    OnNewClient?.Invoke(msg.connectionId);
                    break;
                case Telepathy.EventType.Disconnected:
                    Debug.Log(msg.connectionId + " Disconnected");
                    connectedClients.Remove(msg.connectionId);
                    break;
                case Telepathy.EventType.Data:
                    SNetworkMessage netMsg = JsonUtility.FromJson<SNetworkMessage>(Encoding.UTF8.GetString(msg.data));
                    netMsg.clientID = msg.connectionId;
                    //Debug.Log($"client({msg.connectionId}) has send a {netMsg.type} message.");
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
            Debug.Log($"Client {_message.clientID} asked the Server to spawn {msg.prefabName} and register GUID({msg.GUID}) for everyone.");
            SendNetworkMessageToAllClients(new SNetworkMessage(EMessageType.Instantiate, JsonUtility.ToJson(msg)));
        }

        if (_message.type == EMessageType.UpdateTransform)
        {
            SendNetworkMessageToAllClients(_message);
        }

        //only for Chess
        if (_message.type == EMessageType.Grab || _message.type == EMessageType.Ungrab || _message.type == EMessageType.UpdateGrab || _message.type == EMessageType.UpdateHand)
        {
            SendNetworkMessageToAllClients(_message);
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
                    //Debug.Log($"server has send a {netMsg.type} message.");
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

        if (_message.type == EMessageType.ConnectionSuccessful)
        {
            clientID = int.Parse(_message.JSON);
            Debug.Log($"The Server told me I'm client n°{clientID}");
            OnClientConnected?.Invoke();
            isConnected = true;
        }

        if (_message.type == EMessageType.Instantiate)
        {
            SMessageInstantiate msg = JsonUtility.FromJson<SMessageInstantiate>(_message.JSON);
            InstantiateSyncObject(msg.prefabName, msg.GUID, msg.position, msg.rotation, msg.parentName);
            return;
        }

        if (_message.type == EMessageType.UpdateTransform)
        {
            SMessageUpdateTransform msg = JsonUtility.FromJson<SMessageUpdateTransform>(_message.JSON);
            NetworkSynchronizer.Instance.SyncDown(msg.GUID, msg.position, msg.rotation, msg.scale);
            return;
        }

        //only for Chess
        if (_message.type == EMessageType.Grab)
        {
            NetworkSynchronizer.Instance.GetSMBByGUID(_message.JSON).GetComponent<Piece>().Grab();
        }
        if (_message.type == EMessageType.Ungrab)
        {
            NetworkSynchronizer.Instance.GetSMBByGUID(_message.JSON).GetComponent<Piece>().UnGrab();
        }
        if (_message.type == EMessageType.UpdateGrab)
        {
            SMessageVector3 msg = JsonUtility.FromJson<SMessageVector3>(_message.JSON);
            NetworkSynchronizer.Instance.GetSMBByGUID(msg.GUID).GetComponent<Piece>().SyncDownGrab(msg.vector);
        }
        if (_message.type == EMessageType.UpdateHand)
        {
            NetworkSynchronizer.Instance.SyncHandsDown(JsonUtility.FromJson<SMessageHand>(_message.JSON));
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
    private void InstantiateSyncObject(string _prefabName, string _GUID, Vector3 _position, Quaternion _rotation, string _parentName = null)
    {
        if (string.IsNullOrEmpty(_GUID))
        {
            Debug.LogError("You tried to instantiate a Game Object without a GUID");
            return;
        }
        if (Resources.Load<GameObject>(_prefabName) == null)
        {
            Debug.LogError($"Can't find any {_prefabName} prefab in the Resources Folder");
            return;
        }

        Transform parent = null;
        if (!string.IsNullOrEmpty(_parentName))
        {
            parent = GameObject.Find(_parentName).transform;
        }

        GameObject instance = Instantiate(Resources.Load<GameObject>(_prefabName), _position, _rotation, parent) as GameObject;
        SyncMonoBehaviour SMB = instance.GetComponent<SyncMonoBehaviour>();
        SMB.InitializeGUID(_GUID);
        NetworkSynchronizer.Instance.AddSynchronizeObject(SMB);
        Debug.Log($"Instantiating {_prefabName} under {_parentName} at ({_position}), {_rotation} with GUID({_GUID})", instance);

        if (SMB == null)
        {
            Debug.LogError("You tried to instantiate a prefab which does not have a SyncMonoBehaviour");
            Destroy(instance);
            return;
        }

        OnInstantiate?.Invoke(SMB.gameObject);
    }

    /// <summary>
    /// Called by a client on the client to ask the server to instantiate a GameObject for all clients
    /// </summary>
    /// <param name="_prefabName">The name of the prefab. It should be the exact name of the prefab file that is in the Resource folder.</param>
    /// <param name="_position">The world pos of the instantiated GameObject</param>
    /// <param name="_rotation">The world rotation of the instantiated GameObject</param>
    /// <param name="_parentName">The parent of the instantiated GameObject, can be null</param>
    public void Instantiate(string _prefabName, Vector3 _position, Quaternion _rotation, string _parentName)
    {
        SendNetworkMessageToServer(new SNetworkMessage(EMessageType.Instantiate, JsonUtility.ToJson(new SMessageInstantiate(_prefabName, _position, _rotation, _parentName))));
    }

    [ContextMenu("Start Server")]
    public void StartServer()
    {
        server = new Telepathy.Server();
        server.Start(port);
        isServer = true;
        OnServerStarted?.Invoke();
    }

    [ContextMenu("Start Client")]
    public void StartClient()
    {
        client = new Telepathy.Client();
        client.Connect(serverIP, port);
    }

    public void StartClient(TMPro.TextMeshProUGUI TMP)
    {
        client = new Telepathy.Client();
        if (string.IsNullOrEmpty(TMP.text))
            client.Connect(serverIP, port);
        else
            client.Connect(TMP.text, port);
    }

    public int GetClientID()
    {
        if (clientID == int.MinValue)
        {
            Debug.LogError("You don't have a client ID given by the server !");
        }
        return clientID;
    }

    public int GetNumberOfConnectedClients()
    {
        return connectedClients.Count;
    }
}
