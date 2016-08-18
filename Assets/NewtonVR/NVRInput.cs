using UnityEngine;
using System.Collections;
using System;

using Valve.VR;

namespace NewtonVR
{
    public enum NVRButtonId
    {
        NVR_Button_System = 0,
        NVR_Button_ApplicationMenu = 1,
        NVR_Button_Grip = 2,
        NVR_Button_DPad_Left = 3,
        NVR_Button_DPad_Up = 4,
        NVR_Button_DPad_Right = 5,
        NVR_Button_DPad_Down = 6,
        NVR_Button_A = 7,
        NVR_Button_Axis0 = 8,
        NVR_Button_Axis1 = 9,
        NVR_Button_Axis2 = 10,
        NVR_Button_Axis3 = 11,
        NVR_Button_Axis4 = 12,
        NVR_Button_SteamVR_Touchpad = 13,
        NVR_Button_SteamVR_Trigger = 14,
        NVR_Button_Dashboard_Back = 15,
    }

    public class NVRInput
    {
        IGenericController controller;

        public NVRInput(int index)
        {
            controller = createController();
            controller.Init(index);
        }

        public void TriggerHapticPulse(ushort durationMicroSec = 500, NVRButtonId buttonId = NVRButtonId.NVR_Button_Axis0) { controller.TriggerHapticPulse(durationMicroSec, buttonId); }
        public Vector2 GetAxis(NVRButtonId buttonId = NVRButtonId.NVR_Button_SteamVR_Touchpad) { return controller.GetAxis(buttonId); }

        public bool GetPress(NVRButtonId buttonId) { return controller.GetPress(buttonId); }
        public bool GetPressDown(NVRButtonId buttonId) { return controller.GetPressDown(buttonId); }
        public bool GetPressUp(NVRButtonId buttonId) { return controller.GetPressUp(buttonId); }

        public bool GetTouch(NVRButtonId buttonId) { return controller.GetTouch(buttonId); }
        public bool GetTouchDown(NVRButtonId buttonId) { return controller.GetTouchDown(buttonId); }
        public bool GetTouchUp(NVRButtonId buttonId) { return controller.GetTouchUp(buttonId); }

        IGenericController createController()
        {
            bool useOculus = UnityEngine.VR.VRSettings.loadedDeviceName == "Oculus";
            if (useOculus)
                return new OculusTouchController();

            //default to OpenVR
            return new OpenVRController();
        }
    }

    public class HmdHelper
    {
        static bool dirty = true;
        static uint capabilites = 0;

        public const uint OCULUS_HMD = 1 << 0;
        public const uint HTCVIVE_HMD = 1 << 1;
        public const uint PSVR_HMD = 1 << 2;
        public const uint DUAL_WAND = 1 << 10;
        public const uint SINGLE_WAND = 1 << 11;

        public static bool isOculus()
        {
            resolveConnectedHmdCapabilites();
            return (capabilites & OCULUS_HMD) != 0;
        }

        public static bool isHtcVive()
        {
            resolveConnectedHmdCapabilites();
            return (capabilites & HTCVIVE_HMD) != 0;
        }

        static void resolveConnectedHmdCapabilites()
        {
            if (!dirty)
                return;

            if (UnityEngine.VR.VRSettings.loadedDeviceName.Length == 0)
            {
                capabilites = 0;
                return;
            }

            if (UnityEngine.VR.VRSettings.loadedDeviceName == "Oculus")
            {
                capabilites |= OCULUS_HMD;
                capabilites |= DUAL_WAND; //Assume touch for now
            }

            if (UnityEngine.VR.VRSettings.loadedDeviceName == "OpenVR")
            {
                capabilites |= (HTCVIVE_HMD | DUAL_WAND);
            }

            dirty = false;
        }
    }

    interface IGenericController
    {
        void Init(int idex);

        void TriggerHapticPulse(ushort durationMicroSec, NVRButtonId buttonId);
        Vector2 GetAxis(NVRButtonId buttonId);

        bool GetPress(NVRButtonId buttonId);
        bool GetPressDown(NVRButtonId buttonId);
        bool GetPressUp(NVRButtonId buttonId);

        bool GetTouch(NVRButtonId buttonId);
        bool GetTouchDown(NVRButtonId buttonId);
        bool GetTouchUp(NVRButtonId buttonId);

    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////

    class OpenVRController : IGenericController
    {
        SteamVR_Controller.Device device;

        EVRButtonId[] buttonMapping = {
            EVRButtonId.k_EButton_System,
            EVRButtonId.k_EButton_ApplicationMenu,
            EVRButtonId.k_EButton_Grip,
            EVRButtonId.k_EButton_DPad_Left,
            EVRButtonId.k_EButton_DPad_Up,
            EVRButtonId.k_EButton_DPad_Right,
            EVRButtonId.k_EButton_DPad_Down,
            EVRButtonId.k_EButton_A,
            EVRButtonId.k_EButton_Axis0,
            EVRButtonId.k_EButton_Axis1,
            EVRButtonId.k_EButton_Axis2,
            EVRButtonId.k_EButton_Axis3,
            EVRButtonId.k_EButton_Axis4,
            EVRButtonId.k_EButton_SteamVR_Touchpad,
            EVRButtonId.k_EButton_SteamVR_Trigger,
            EVRButtonId.k_EButton_Dashboard_Back,
            EVRButtonId.k_EButton_Max
        };

        void IGenericController.Init(int index)
        {
            device = SteamVR_Controller.Input(index);
        }

        void IGenericController.TriggerHapticPulse(ushort durationMicroSec, NVRButtonId buttonId)
        {
            device.TriggerHapticPulse(durationMicroSec, buttonMapping[(int)buttonId]);
        }

        Vector2 IGenericController.GetAxis(NVRButtonId buttonId)
        {
            return device.GetAxis(buttonMapping[(int)buttonId]);
        }

        bool IGenericController.GetPress(NVRButtonId buttonId)
        {
            return device.GetPress(buttonMapping[(int)buttonId]);
        }

        bool IGenericController.GetPressDown(NVRButtonId buttonId)
        {
            return device.GetPressDown(buttonMapping[(int)buttonId]);
        }

        bool IGenericController.GetPressUp(NVRButtonId buttonId)
        {
            return device.GetPressUp(buttonMapping[(int)buttonId]);
        }

        bool IGenericController.GetTouch(NVRButtonId buttonId)
        {
            return device.GetTouch(buttonMapping[(int)buttonId]);
        }

        bool IGenericController.GetTouchDown(NVRButtonId buttonId)
        {
            return device.GetTouchDown(buttonMapping[(int)buttonId]);
        }

        bool IGenericController.GetTouchUp(NVRButtonId buttonId)
        {
            return device.GetTouchUp(buttonMapping[(int)buttonId]);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////

    class OculusTouchController : IGenericController
    {
		OVRInput.Button[] buttonMapping = {
			OVRInput.Button.None, //NVR_Button_System = 0,
			OVRInput.Button.None, //NVR_Button_ApplicationMenu = 1,
			OVRInput.Button.PrimaryHandTrigger, //NVR_Button_Grip = 2,
			OVRInput.Button.DpadLeft, //NVR_Button_DPad_Left = 3,
			OVRInput.Button.DpadUp, //NVR_Button_DPad_Up = 4,
			OVRInput.Button.DpadRight, //NVR_Button_DPad_Right = 5,
			OVRInput.Button.DpadDown, //NVR_Button_DPad_Down = 6,
			OVRInput.Button.None, //NVR_Button_A = 7,
			OVRInput.Button.None, //NVR_Button_Axis0 = 8,
			OVRInput.Button.None, //NVR_Button_Axis1 = 9,
			OVRInput.Button.None, //NVR_Button_Axis2 = 10,
			OVRInput.Button.None, //NVR_Button_Axis3 = 11,
			OVRInput.Button.None, //NVR_Button_Axis4 = 12,
			OVRInput.Button.None, //NVR_Button_SteamVR_Touchpad = 13,
			OVRInput.Button.PrimaryIndexTrigger, //NVR_Button_SteamVR_Trigger = 14,
			OVRInput.Button.None, //NVR_Button_Dashboard_Back = 15,
		};

		OVRInput.Axis2D[] axisMapping = {
			OVRInput.Axis2D.None, //NVR_Button_System = 0,
			OVRInput.Axis2D.None, //NVR_Button_ApplicationMenu = 1,
			OVRInput.Axis2D.None, //NVR_Button_Grip = 2,
			OVRInput.Axis2D.None, //NVR_Button_DPad_Left = 3,
			OVRInput.Axis2D.None, //NVR_Button_DPad_Up = 4,
			OVRInput.Axis2D.None, //NVR_Button_DPad_Right = 5,
			OVRInput.Axis2D.None, //NVR_Button_DPad_Down = 6,
			OVRInput.Axis2D.None, //NVR_Button_A = 7,
			OVRInput.Axis2D.PrimaryThumbstick, //NVR_Button_Axis0 = 8,
			OVRInput.Axis2D.None, //NVR_Button_Axis1 = 9,
			OVRInput.Axis2D.None, //NVR_Button_Axis2 = 10,
			OVRInput.Axis2D.None, //NVR_Button_Axis3 = 11,
			OVRInput.Axis2D.None, //NVR_Button_Axis4 = 12,
			OVRInput.Axis2D.None, //NVR_Button_SteamVR_Touchpad = 13,
			OVRInput.Axis2D.None, //NVR_Button_SteamVR_Trigger = 14,
			OVRInput.Axis2D.None, //NVR_Button_Dashboard_Back = 15,
		};

        HapticPulseHelper hapticPulseHelper = null;

        class HapticPulseHelper : MonoBehaviour
        {
            IEnumerator performHapticPulse(float millis, OVRInput.Controller controllerType)
            {
                float frequency = 0.5f;
                float amplitude = 0.5f;
                OVRInput.SetControllerVibration(frequency, amplitude, controllerType);
                yield return new WaitForSeconds(millis / 1000.0f);
                OVRInput.SetControllerVibration(0.0f, 0.0f, controllerType);
            }

            public void TriggerHapticPulse(ushort durationMicroSec, NVRButtonId buttonId, OVRInput.Controller controllerType)
            {
                StartCoroutine(performHapticPulse(durationMicroSec / 1000.0f, controllerType));
            }
        };

        NVRHandOculus.HandednessId handness;
        OVRInput.Controller controllerType = OVRInput.Controller.None;

        void IGenericController.Init(int index)
        {
            handness = (NVRHandOculus.HandednessId) index;
            controllerType = (handness == NVRHandOculus.HandednessId.Left) ? OVRInput.Controller.LTouch : OVRInput.Controller.RTouch;

            GameObject go = new GameObject();
            hapticPulseHelper = go.AddComponent<HapticPulseHelper>();
        }


        void IGenericController.TriggerHapticPulse(ushort durationMicroSec, NVRButtonId buttonId)
        {
            hapticPulseHelper.TriggerHapticPulse(durationMicroSec, buttonId, controllerType);
        }

        Vector2 IGenericController.GetAxis(NVRButtonId buttonId)
        {
            return OVRInput.Get(axisMapping[(int)buttonId], controllerType);
        }

        bool IGenericController.GetPress(NVRButtonId buttonId)
        {
            return OVRInput.Get(buttonMapping[(int)buttonId], controllerType);
        }

        bool IGenericController.GetPressDown(NVRButtonId buttonId)
        {
            return OVRInput.GetDown(buttonMapping[(int)buttonId], controllerType);
        }

        bool IGenericController.GetPressUp(NVRButtonId buttonId)
        {
			return OVRInput.GetUp(buttonMapping[(int)buttonId], controllerType);
        }

        bool IGenericController.GetTouch(NVRButtonId buttonId)
        {
            return false;
        }

        bool IGenericController.GetTouchDown(NVRButtonId buttonId)
        {
            return false;
        }

        bool IGenericController.GetTouchUp(NVRButtonId buttonId)
        {
            return false;
        }
    }
}
