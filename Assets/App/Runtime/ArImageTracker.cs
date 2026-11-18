using System;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace CardapioRA
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public sealed class ArImageTracker : MonoBehaviour
    {
        public event Action<string, Transform> MarkerTracked;
        public event Action<string> MarkerLost;
        private ARTrackedImageManager manager;
        private string activeMarkerId;

        private void Awake() => manager = GetComponent<ARTrackedImageManager>();

        private void OnEnable() => manager.trackablesChanged.AddListener(OnImagesChanged);
        private void OnDisable() => manager.trackablesChanged.RemoveListener(OnImagesChanged);

        private void OnImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
        {
            foreach (var image in args.added)
                PublishTrackingState(image);
            foreach (var image in args.updated)
                PublishTrackingState(image);
            foreach (var image in args.removed)
                PublishLost(image.Value.referenceImage.name);
        }

        private void PublishTrackingState(ARTrackedImage image)
        {
            if (image.trackingState == TrackingState.Tracking)
            {
                activeMarkerId = image.referenceImage.name;
                MarkerTracked?.Invoke(image.referenceImage.name, image.transform);
            }
            else
            {
                PublishLost(image.referenceImage.name);
            }
        }

        private void PublishLost(string markerId)
        {
            if (activeMarkerId != markerId)
                return;

            activeMarkerId = null;
            MarkerLost?.Invoke(markerId);
        }
    }
}

