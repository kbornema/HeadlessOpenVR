using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

namespace HeadlessOpenVR
{
    public class HOVR_TrackerInfo
    {
        private int _deviceId = -1;
        public int DeviceId { get { return _deviceId; } }

        private ETrackedDeviceClass _deviceClass;
        public ETrackedDeviceClass DeviceClass { get { return _deviceClass; } }

        public string SerialNumber = "";

        public string AdditionalString;

        public bool HasBattery = false;

        public HOVR_TrackerInfo(int deviceId, ETrackedDeviceClass deviceClass)
        {
            _deviceId = deviceId;
            _deviceClass = deviceClass;
        }

        public override string ToString()
        {
            return _deviceClass.ToString() + " (" + SerialNumber + ") at DeviceId: " + _deviceId + ", " + AdditionalString;
        }
    }
}