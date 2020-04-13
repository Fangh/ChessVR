using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncMonoBehaviour : MonoBehaviour, ITransformSync
{
    public string GUID
    {
        get { return guid;  }
        private set { guid = value; }
    }

    private string guid;
    private float timeSinceLastSyncDown;
    private Vector3 lastPos;
    private Quaternion lastRot;
    private Vector3 lastScale;

    private void Awake()
    {
        GUID = GetInstanceID().ToString();
    }

    public void InitializeMyGUID(string _GUID)
    {
        GUID = _GUID;
    }

    public virtual void SyncTransform(Vector3 _position, Quaternion _rotation, Vector3 _scale)
    {
        transform.position = _position;
        transform.rotation = _rotation;
        transform.localScale = _scale;
        timeSinceLastSyncDown = Time.time;
    }

    public virtual void Update()
    {
        if (Time.time > timeSinceLastSyncDown + NetworkSynchronizer.Instance.synchroTime) //Synchro Up only if you hasn't been Sync Down recently
        {
            if(lastPos != transform.position || lastRot != transform.rotation || lastScale != transform.localScale)
            {
                NetworkSynchronizer.Instance.SyncUp(this);
            }
        }
    }

    public virtual void LateUpdate()
    {
        lastPos = transform.position;
        lastRot = transform.rotation;
        lastScale = transform.localScale;
    }
}