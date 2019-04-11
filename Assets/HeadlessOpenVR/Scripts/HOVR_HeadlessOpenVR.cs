using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace HeadlessOpenVR
{   
    public class HOVR_HeadlessOpenVR : MonoBehaviour
    {
        private CVRSystem _vrSystem;

        [SerializeField]
        private ETrackingUniverseOrigin _trackingOrigin = ETrackingUniverseOrigin.TrackingUniverseStanding;
        [SerializeField]
        private Vector3 _posScale = new Vector3(2.0f, 2.0f, -2.0f);
        [SerializeField]
        private bool _debugLogs;
        [SerializeField]
        private List<HOVR_Tracker> _tracker;

        private TrackedDevicePose_t[] _allPoses;

        private void Awake()
        {
            _allPoses = new TrackedDevicePose_t[OpenVR.k_unMaxTrackedDeviceCount];

            var error = EVRInitError.None;
            _vrSystem = OpenVR.Init(ref error, EVRApplicationType.VRApplication_Other);

            if (error != EVRInitError.None)
            {
                Debug.LogWarning(error, gameObject);
            }

            InitOpenVrDevices();

            UpdatePoses();
        }

        private void InitOpenVrDevices()
        {
            if (_vrSystem == null)
                return;

            List<HOVR_TrackerInfo> trackerDescriptions = new List<HOVR_TrackerInfo>();

            for (uint i = 0; i < OpenVR.k_unMaxTrackedDeviceCount; i++)
            {
                var deviceClass = _vrSystem.GetTrackedDeviceClass(i);

                if (deviceClass != ETrackedDeviceClass.Invalid)
                {
                    HOVR_TrackerInfo tmpTracker = new HOVR_TrackerInfo((int)i, deviceClass);

                    if (deviceClass == ETrackedDeviceClass.TrackingReference)
                    {
                        var modeLabel = GetDevicePropertyString(i, ETrackedDeviceProperty.Prop_ModeLabel_String, 16);
                        tmpTracker.AdditionalString = modeLabel;
                    }

                    var serialNumber = GetDevicePropertyString(i, ETrackedDeviceProperty.Prop_SerialNumber_String, 32);
                    tmpTracker.SerialNumber = serialNumber;

                    var batteryError = ETrackedPropertyError.TrackedProp_UnknownProperty;
                    _vrSystem.GetFloatTrackedDeviceProperty(i, ETrackedDeviceProperty.Prop_DeviceBatteryPercentage_Float, ref batteryError);

                    if (batteryError == ETrackedPropertyError.TrackedProp_Success)
                        tmpTracker.HasBattery = true;

                    trackerDescriptions.Add(tmpTracker);
                }
            }

            for (int i = 0; i < trackerDescriptions.Count; i++)
            {
                var curInfo = trackerDescriptions[i];
                var tracker = GetValidTracker(curInfo);

                if (tracker != null)
                {
                    if (tracker.DeviceId == HOVR_Tracker.UNINIT_DEVICE_ID)
                    {
                        tracker.Init(curInfo);

                        if (_debugLogs)
                            Debug.Log("Found OVR Device: " + curInfo.ToString(), gameObject);
                    }

                    else if (_debugLogs)
                    {
                        Debug.LogWarning("Found the same ViveTracker for multiple different detected OVR devices for info: " + curInfo.ToString(), gameObject);
                    }
                }
                else if (_debugLogs)
                {
                    Debug.LogWarning("Could not find ViveTracker for: " + curInfo.ToString(), gameObject);
                }
            }
        }

        private string GetDevicePropertyString(uint deviceId, ETrackedDeviceProperty prop, uint capacity)
        {
            ETrackedPropertyError error = ETrackedPropertyError.TrackedProp_UnknownProperty;

            System.Text.StringBuilder strBuilder = new System.Text.StringBuilder((int)capacity);
            _vrSystem.GetStringTrackedDeviceProperty(deviceId, prop, strBuilder, capacity, ref error);

            if (error == ETrackedPropertyError.TrackedProp_Success)
                return strBuilder.ToString().Trim();

            return "";
        }

        private HOVR_Tracker GetValidTracker(HOVR_TrackerInfo info)
        {
            var validTrackers = _tracker.FindAll(x => x.DeviceClass == info.DeviceClass);

            if (validTrackers.Count == 1)
                return validTrackers[0];

            var foundPerSerial = validTrackers.Find(x => x.SerialNumber.Length > 0 && x.SerialNumber == info.SerialNumber);

            if (foundPerSerial != null)
                return foundPerSerial;

            return null;
        }

        private void Update()
        {
            if (_vrSystem == null)
                return;

            if (_tracker.Count == 0)
                return;

            UpdatePoses();
        }

        private void UpdatePoses()
        {
            _vrSystem.GetDeviceToAbsoluteTrackingPose(_trackingOrigin, 0, _allPoses);

            for (int i = 0; i < _tracker.Count; i++)
            {
                int index = _tracker[i].DeviceId;

                if (index == -1)
                    continue;

                var pose = _allPoses[index];

                float batteryPercent = 1.0f;
                bool isCharging = false;

                if (_tracker[i].HasBattery)
                {
                    var error = ETrackedPropertyError.TrackedProp_UnknownProperty;
                    batteryPercent = _vrSystem.GetFloatTrackedDeviceProperty((uint)index, ETrackedDeviceProperty.Prop_DeviceBatteryPercentage_Float, ref error);

                    isCharging = _vrSystem.GetBoolTrackedDeviceProperty((uint)index, ETrackedDeviceProperty.Prop_DeviceIsCharging_Bool, ref error);


                }

                _tracker[i].SetTrackingState(pose.eTrackingResult, batteryPercent, isCharging, false);

                if (pose.bPoseIsValid)
                {
                    var absTracking = pose.mDeviceToAbsoluteTracking;

                    Vector3 pos;
                    Quaternion rot;
                    HOVR_Utility.GetPosAndRotation(absTracking, out pos, out rot);

                    pos.Scale(_posScale);

                    _tracker[i].ReceivePose(pos, rot);
                }
            }
        }
    }
}