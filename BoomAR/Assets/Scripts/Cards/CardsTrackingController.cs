using GrowAR.Characters;
using GrowAR.Characters.Models;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Zenject;

namespace GrowAR.Cards
{
    [RequireComponent(typeof(ARTrackedImageManager))]
    public class CardsTrackingController : MonoBehaviour
    {
        [SerializeField] private GameObject _cardsCollection;

        private Dictionary<string, GameObject> _createdPrefabs
            = new Dictionary<string, GameObject>();
        private Dictionary<string, GameObject> _usedCardsCollection
            = new Dictionary<string, GameObject>();
        private ARTrackedImageManager _arTrackedImageManager;

        private CardCharacterController _characterController;

        [Inject]
        public void Construct(SignalBus signalBus, CardCharacterController characterController)
        {
            _characterController = characterController;

            _characterController.RemoveCards += RemoveUsedCard;
        }

        private void RemoveUsedCard(CardCharacterInconstantModel characterInconstantModel)
        {
            if (_createdPrefabs.ContainsKey(characterInconstantModel.CharacterId))
            {
                _usedCardsCollection.Add(characterInconstantModel.CharacterId, _createdPrefabs[characterInconstantModel.CharacterId]);
                _createdPrefabs.Remove(characterInconstantModel.CharacterId);
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            _arTrackedImageManager = FindObjectOfType<ARTrackedImageManager>();


            foreach (Transform card in _cardsCollection.transform)
            {
                _createdPrefabs.Add(card.name, card.gameObject);
                card.gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            _arTrackedImageManager.trackedImagesChanged += TrackedImageChanged;
        }

        private void OnDisable()
        {
            _arTrackedImageManager.trackedImagesChanged -= TrackedImageChanged;
        }
        private void TrackedImageChanged(ARTrackedImagesChangedEventArgs eventArgsObj)
        {
            foreach (ARTrackedImage aRTrackedImage in eventArgsObj.added)
            {
                UpdateImage(aRTrackedImage);
            }
            foreach (ARTrackedImage aRTrackedImage in eventArgsObj.removed)
            {
                _createdPrefabs[aRTrackedImage.name].SetActive(false);
            }
            foreach (ARTrackedImage aRTrackedImage in eventArgsObj.updated)
            {
                UpdateImage(aRTrackedImage);
            }
        }

        private void UpdateImage(ARTrackedImage aRTrackedImage)
        {
            string name = aRTrackedImage.referenceImage.name;
            Vector3 pos = aRTrackedImage.transform.position;

            GameObject prefab;
            if (_createdPrefabs.ContainsKey(name))
            {
                prefab = _createdPrefabs[name];
                prefab.transform.position = pos;
                prefab?.SetActive(true);
            }
        }
    }
}
