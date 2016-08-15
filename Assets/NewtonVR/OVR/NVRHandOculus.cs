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

    void Start ()
    {
        OVRCameraRig cameraRig = GameObject.FindObjectOfType<OVRCameraRig>();
        if (cameraRig != null)
        {
            trackedTransform = (handedness == HandednessId.Left) ? cameraRig.leftHandAnchor : cameraRig.rightHandAnchor;
            BroadcastMessage("SetDeviceIndex", (int)handedness, SendMessageOptions.DontRequireReceiver); //Simulate SteamVR behaviour
        }
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
