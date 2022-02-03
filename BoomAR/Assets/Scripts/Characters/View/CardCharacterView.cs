using GrowAR.Characters.Models;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static GrowAR.Characters.Infrastructure.CardCharactersSignals;

namespace GrowAR.Characters.View
{
    public class CardCharacterView : MonoBehaviour
    {
        [SerializeField] private Slider _healthBar;
        [SerializeField] private Slider _attackPowerBar;

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
            // 
            _characterController.CardsCollided += UpdateView;

        }

        private void OnCollisionEnter(Collision collision)
        {
            _signalBus.Fire(new CardCharacterCollidedSignal()
            {
                CharacterId = this.gameObject.name,
                CollidedCharacterId = collision.gameObject.name
            });

        }

        private void UpdateView(CardCharacterInconstantModel firstCharacterInconstantModel,
            CardCharacterInconstantModel secondCharacterInconstantModel)
        {
            if (firstCharacterInconstantModel.CharacterId == this.gameObject.name)
            {
                if (firstCharacterInconstantModel.CurrentHealth <= 0)
                {
                    this.gameObject.SetActive(false);
                }
            }
            if (secondCharacterInconstantModel.CharacterId == this.gameObject.name)
            {
                if (secondCharacterInconstantModel.CurrentHealth <= 0)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }
}
