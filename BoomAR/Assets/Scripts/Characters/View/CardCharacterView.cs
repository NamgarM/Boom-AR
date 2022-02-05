using GrowAR.Characters.Models;
using TMPro;
using UnityEngine;
using Zenject;
using static GrowAR.Characters.Infrastructure.CardCharactersSignals;

namespace GrowAR.Characters.View
{
    public class CardCharacterView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _healthIndicator;
        [SerializeField] private TextMeshProUGUI _energyIndicator;
        [Space]
        [Space]
        [SerializeField] private AnimateCharacterAppearance _animateCharacterAppearance;
        [SerializeField] private AnimateCharacterCollision _animateCharacterCollision;
        [SerializeField] private Material[] _characterMaterials;

        private float _animationAmountPerSecond = 0.25f;
        private bool _isAnimationPlayed = false;

        private CardCharacterController _characterController;
        private SignalBus _signalBus;

        // Load collection of CharacterConstantModel
        // Search (CharacterConstantModel) id of the card by name
        // Put it new created non constant (for this particular character?)
        [Inject]
        public void Construct(SignalBus signalBus, CardCharacterController characterController)
        {
            _characterController = characterController;
            _signalBus = signalBus;

            // Will be good to make rule - set data with name ?
            //Load and search constant
            CardCharacterConstantModel characterConstantModel = _characterController._characterConstantModels.Find(c => c.CharacterId == this.name);
            // Create inconstant
            CardCharacterInconstantModel characterInconstantModel = new CardCharacterInconstantModel();
            // Assign
            characterInconstantModel.CharacterId = characterConstantModel.CharacterId;
            characterInconstantModel.CurrentEnergy = characterConstantModel.StartingEnergy;
            characterInconstantModel.CurrentHealth = characterConstantModel.StartingHealth;

            // Save
            _characterController._characterInconstantModels.Add(characterInconstantModel);
            CardCharacterModel cardCharacterModel = new CardCharacterModel()
            {
                CharacterConstantModel = characterConstantModel,
                CharacterInconstantModel = characterInconstantModel
            };
            _characterController
                .CardCharacterModels
                .Add(characterConstantModel.CharacterId, cardCharacterModel);

            _characterController.SetCharacterView(this);

            UpdateView(null, characterInconstantModel, null);
            _characterController.CardsCollided += UpdateView;
            _animateCharacterAppearance?.Initialize(_characterMaterials);

        }

        private void Update()
        {
            if (_isAnimationPlayed == false && _animateCharacterAppearance != null)
                _isAnimationPlayed = _animateCharacterAppearance
                    .IsAppearingAnimationShowed(_characterMaterials, _animationAmountPerSecond);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.rigidbody != null)
                collision.rigidbody.isKinematic = true;

            _signalBus.Fire(new CardCharacterCollidedSignal()
            {
                CharacterId = this.gameObject.name,
                CollidedCharacterId = collision.gameObject.name,
                OpponentObject = collision.gameObject
            });
        }

        private void UpdateView(string animationType, 
            CardCharacterInconstantModel characterInconstantModel,
            GameObject opponent)
        {
            // Play animation
            switch (animationType)
            {
                case "Healing":
                    _animateCharacterCollision?
                        .ShowCollisionAnimation(opponent.transform.position, null, _healthIndicator
                        .transform.parent.gameObject);
                    break;
                case null:
                    break;
            }

            // Update Healthbar
            _energyIndicator.text =
                characterInconstantModel.CurrentEnergy.ToString();
            _healthIndicator.text =
                characterInconstantModel.CurrentHealth.ToString();;
        }
    }
}
