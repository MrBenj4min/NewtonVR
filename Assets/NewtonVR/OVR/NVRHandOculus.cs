using UnityEngine;
using System.Collections;

public class NVRHandOculus : MonoBehaviour
{
    public enum HandednessId : int
    {
        Left = 0,
        Right = 1,
    }

    public HandednessId handedness;

    Transform trackedTransform = null;

    static bool[] initialized = new bool[2];

    void Start ()
    {
        int handIdx = (int)handedness;

        OVRCameraRig cameraRig = GameObject.FindObjectOfType<OVRCameraRig>();
        if (cameraRig != null)
        {
            trackedTransform = (handedness == HandednessId.Left) ? cameraRig.leftHandAnchor : cameraRig.rightHandAnchor;
            if (NVRHandOculus.initialized[handIdx] == false) // prevent this message from being sent more than one per hand
                BroadcastMessage("SetDeviceIndex", (int)handedness, SendMessageOptions.DontRequireReceiver); //Simulate SteamVR behaviour
        }

        NVRHandOculus.initialized[handIdx] = true;
    }

    void LateUpdate ()
    {
        if (trackedTransform != null)
        {
            transform.position = trackedTransform.position;
            transform.rotation = trackedTransform.rotation;
        }
    }
}
