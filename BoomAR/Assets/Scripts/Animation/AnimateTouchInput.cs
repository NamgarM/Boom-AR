using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace GrowAR.CardsCollectionMenu
{
    public class AnimateTouchInput : MonoBehaviour
    {
        [SerializeField] private GameObject _particleEffectPrefab;

        private ARRaycastManager _arRaycastManager;
        private Camera _mainCamera;
        static List<ARRaycastHit> hits = new List<ARRaycastHit>();

        void Start()
        {
            _mainCamera = Camera.main;
            _arRaycastManager = _mainCamera.GetComponentInParent<ARRaycastManager>();
        }

        void Update()
        {
            if (Input.touchCount > 0)
                SpawnParticleEffect();
        }

        private void SpawnParticleEffect()
        {
            Touch touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    if (_arRaycastManager
                        .Raycast(touch.position, hits, TrackableType.All))
                    {
                        GameObject particleEffect =
                            (GameObject)Instantiate(_particleEffectPrefab);
                        var hitPose = hits[0].pose;
                        particleEffect.transform.position =
                            hitPose.position;
                    }
                    break;
            }
        }
    }
}
