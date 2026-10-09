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
        private ARTrackedImageManager manager;

        private void Awake() => manager = GetComponent<ARTrackedImageManager>();

        private void OnEnable() => manager.trackablesChanged.AddListener(OnImagesChanged);
        private void OnDisable() => manager.trackablesChanged.RemoveListener(OnImagesChanged);

        private void OnImagesChanged(ARTrackablesChangedEventArgs<ARTrackedImage> args)
        {
            foreach (var image in args.added)
                PublishWhenTracking(image);
            foreach (var image in args.updated)
                PublishWhenTracking(image);
        }

        private void PublishWhenTracking(ARTrackedImage image)
        {
            if (image.trackingState == TrackingState.Tracking)
                MarkerTracked?.Invoke(image.referenceImage.name, image.transform);
        }
    }
}

