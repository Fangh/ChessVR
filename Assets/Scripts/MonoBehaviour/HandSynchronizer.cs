using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandSynchronizer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    private Dictionary<string, Transform> leftHandBones = new Dictionary<string, Transform>();
    private Dictionary<string, Transform> rightHandBones = new Dictionary<string, Transform>();

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1.0f);

        Transform leftRoot = leftHand.FindChildRecursive("Hand_Start");
        Transform rightRoot = rightHand.FindChildRecursive("Hand_Start");

        foreach (Transform child in leftRoot.GetComponentsInChildren<Transform>())
        {
            Debug.Log($"Adding left bone {child.name}", child);
            if (!leftHandBones.ContainsKey(child.name))
                leftHandBones.Add(child.name, child);
        }

        foreach (Transform child in rightRoot.GetComponentsInChildren<Transform>())
        {
            Debug.Log($"Adding right bone {child.name}", child);
            if (!rightHandBones.ContainsKey(child.name))
                rightHandBones.Add(child.name, child);
        }
    }

    public void SyncHandDown(SMessageHand _data)
    {
        if (_data.ownerClientID == NetworkManager.Instance.GetClientID())
            return;

        if (_data.handType == 0)
        {
            leftHand.position = _data.position;
            leftHand.rotation = _data.rotation;

            foreach (var bone in _data.bones)
            {
                if (leftHandBones.ContainsKey(bone.name))
                    leftHandBones[bone.name].transform.rotation = bone.rotation;
            }
        }
        if (_data.handType == 1)
        {
            rightHand.position = _data.position;
            rightHand.rotation = _data.rotation;

            foreach (var bone in _data.bones)
            {
                if (rightHandBones.ContainsKey(bone.name))
                    rightHandBones[bone.name].transform.rotation = bone.rotation;
            }
        }
    }

    public void SyncHandUp()
    {
        NetworkManager.Instance.SendNetworkMessageToServer(new SNetworkMessage(EMessageType.UpdateHand, JsonUtility.ToJson(CreateMessageForHand(0))));
        NetworkManager.Instance.SendNetworkMessageToServer(new SNetworkMessage(EMessageType.UpdateHand, JsonUtility.ToJson(CreateMessageForHand(1))));
    }

    private SMessageHand CreateMessageForHand(int _type)
    {
        SMessageHand message = new SMessageHand(_type, new List<SBone>(), _type == 0 ? leftHand.position : rightHand.position, _type == 0 ? leftHand.rotation : rightHand.rotation);
        message.ownerClientID = NetworkManager.Instance.GetClientID();
        foreach (var bone in _type == 0 ? leftHandBones : rightHandBones)
        {
            message.bones.Add(new SBone(bone.Key, bone.Value.rotation));
        }
        return message;
    }
}
