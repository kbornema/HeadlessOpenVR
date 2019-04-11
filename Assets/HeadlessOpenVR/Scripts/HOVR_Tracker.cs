using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Valve.VR;

namespace HeadlessOpenVR
{
    public class HOVR_Tracker : MonoBehaviour
    {   
        [System.Serializable]
        public class TrackerEvent : UnityEvent<HOVR_Tracker> { }

        public const int UNINIT_DEVICE_ID = -1;
#if UNITY_EDITOR
        [TextArea(1, 4)]
        public string DebugText;
#endif

        [SerializeField]
        private Transform _pivot;

        [SerializeField]
        private ETrackedDeviceClass _deviceClass;
        public ETrackedDeviceClass DeviceClass { get { return _deviceClass; } }

        [SerializeField]
        private string _serialNumber;
        public string SerialNumber { get { return _serialNumber; } }

        [SerializeField, HOVR_ReadOnly]
        private Valve.VR.ETrackingResult _trackingState = ETrackingResult.Uninitialized;
        public Valve.VR.ETrackingResult GetTrackingState() { return _trackingState; }

        [HideInInspector]
        public TrackerEvent OnTrackerStateChangedEvent = new TrackerEvent();

        private int _deviceId = -1;
        public int DeviceId { get { return _deviceId; } }

        private bool _hasBattery = false;
        public bool HasBattery { get { return _hasBattery; } }

        private float _batteryPercent = 1.0f;
        public float BatteryPercent { get { return _batteryPercent; } }

        private bool _isCharging = false;
        public bool IsCharging { get { return _isCharging; } }

        private void Reset()
        {
            if (!_pivot)
                _pivot = transform;
        }

        public void Init(HOVR_TrackerInfo info)
        {
            _deviceId = info.DeviceId;
            _hasBattery = info.HasBattery;
        }

        public void ReceivePose(Vector3 pos, Quaternion rotation)
        {
            _pivot.localPosition = pos;
            _pivot.localRotation = rotation;
        }

        /// <summary> Forces <see cref="OnTrackerStateChangedEvent"/> to be fired. </summary>
        public void BroadcastTrackingState()
        {
            SetTrackingState(_trackingState, _batteryPercent, true);
        }

        public void SetTrackingState(Valve.VR.ETrackingResult state, float batteryPercent, bool isCharging, bool force = false)
        {
            bool changed = false;

            //only update if the battery changed enough:
            const float MIN_BATTERY_CHANGE_PERCENT = 0.01f;
            if (Mathf.Abs(_batteryPercent - batteryPercent) > MIN_BATTERY_CHANGE_PERCENT)
            {
                _batteryPercent = batteryPercent;
                changed = true;
            }

            if (_trackingState != state)
            {
                _trackingState = state;
                changed = true;
            }

            if(isCharging != _isCharging)
            {
                _isCharging = isCharging;
                changed = true;
            }

            if (force || changed)
                OnTrackerStateChangedEvent.Invoke(this);
        }
    }
}