using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

namespace HeadlessOpenVR
{
    public class HOVR_UI_TrackerState : MonoBehaviour
    {
        private static Color Orange = new Color(1.0f, 0.525f, 0.01f);

        [SerializeField]
        private HOVR_Tracker _tracker;

        [SerializeField]
        private Image _stateImage;
        [SerializeField]
        private Image _batteryImage;
        [SerializeField]
        private Image _chargeImage;

        // Use this for initialization
        private void Awake()
        {
            if (!_tracker)
            {
                Debug.LogWarning("No " + typeof(HOVR_Tracker).Name + " is assigned to " + GetType().Name + ". Ignoring it.", gameObject);
                return;
            }

            else
            {
                _tracker.OnTrackerStateChangedEvent.AddListener(OnTrackerStateChanged);
                _tracker.BroadcastTrackingState();
            }
        }

        private void OnTrackerStateChanged(HOVR_Tracker arg0)
        {
            SetTrackingStateColor(arg0.GetTrackingState());
            SetBatteryPercent(arg0.BatteryPercent);

            if (_chargeImage)
                _chargeImage.enabled = arg0.IsCharging;
        }

        private void SetTrackingStateColor(ETrackingResult eTrackingResult)
        {
            if (_stateImage)
            {
                switch (eTrackingResult)
                {
                    case ETrackingResult.Uninitialized:
                        _stateImage.color = Color.red;
                        break;
                    case ETrackingResult.Calibrating_OutOfRange:
                        _stateImage.color = Orange;
                        break;
                    case ETrackingResult.Calibrating_InProgress:
                        _stateImage.color = Color.yellow;
                        break;
                    case ETrackingResult.Running_OutOfRange:
                        _stateImage.color = Color.cyan;
                        break;
                    case ETrackingResult.Running_OK:
                        _stateImage.color = Color.green;
                        break;
                    default:
                        _stateImage.color = Color.magenta;
                        break;
                }
            }
        }

        private void SetBatteryPercent(float batteryPercent)
        {
            if (_batteryImage)
            {
                _batteryImage.color = Color.Lerp(Color.red, Color.green, batteryPercent);
                _batteryImage.fillAmount = batteryPercent;
            }
        }
    }
}