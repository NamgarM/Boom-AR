using GrowAR.Animation;
using GrowAR.Characters.Models;
using GrowAR.Sound;
using System;
using System.Collections;
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
        [SerializeField] private Transform _character;
        [SerializeField] private Material[] _characterMaterials;
        [SerializeField] private SoundManager _soundManager;
        [SerializeField] private Animator _animator;


        private float _animationAmountPerSecond = 0.25f;
        private bool _isAnimationPlayed = true;
        private float _statsIndicatorsAnimationSpeed = 1f;

        private CardCharacterController _characterController;
        private SignalBus _signalBus;


        // Load collection of CharacterConstantModel
        // Search (CharacterConstantModel) id of the card by name
        // Put it new created non constant (for this particular character?)
        [Inject]
        public void Construct(AnimateCharacterCollision animateCharacterCollision, SignalBus signalBus, CardCharacterController characterController)
        {
            _animateCharacterCollision = animateCharacterCollision;
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

            // UI and animation
            _energyIndicator.text =
                characterInconstantModel.CurrentEnergy.ToString();
            _healthIndicator.text =
                characterInconstantModel.CurrentHealth.ToString(); ;
            _characterController.CardsCollided += UpdateView;
            _animateCharacterAppearance?.Initialize(_characterMaterials);

        }

        private void Update()
        {
            if (_animateCharacterAppearance != null && !_isAnimationPlayed)
            {
                _isAnimationPlayed = _animateCharacterAppearance
                    .IsAppearingAnimationShowed(_characterMaterials, _animationAmountPerSecond);
            }
        }

        private void OnEnable()
        {
            _isAnimationPlayed = false;
            _soundManager?.Play("DissolveSFX", 0.5f);
        }

        private void OnApplicationQuit()
        {
            _animateCharacterAppearance?.ResetDissolveAnimation(_characterMaterials);
        }


        private void OnCollisionEnter(Collision collision)
        {
            if (collision.rigidbody != null)
                collision.rigidbody.isKinematic = true;

            _soundManager?.Play("LongHealingProcessSFX", 1f);
            _signalBus.Fire(new CardCharacterCollidedSignal()
            {
                CharacterId = this.gameObject.name,
                CollidedCharacterId = collision.gameObject.name,
                OpponentObject = collision.gameObject,
                CardCharacterView = this
            });
        }


        private void UpdateView(string animationType,
            CardCharacterInconstantModel characterInconstantModel,
            GameObject opponent,
            CardCharacterInconstantModel prevCardCharacterInconstantModel)
        {
            if (opponent != this.gameObject && this.gameObject.activeInHierarchy)
            {
                // Play animation
                switch (animationType)
                {
                    case "Heal":
                        _animateCharacterCollision?
                            .ShowHealingCollisionAnimation(opponent.transform.position, null, _healthIndicator
                            .transform.parent.gameObject, _character);
                        break;
                    case "Attack":
                        this.transform
                            .LookAt(new Vector3(opponent.transform.position.x, this.transform.position.y, this.transform.position.z));
                        _animateCharacterCollision?
                            .ShowShootingAnimation(_animator);
                        break;
                }

                CheckStatsIndicators(characterInconstantModel, prevCardCharacterInconstantModel);
            }
        }

        private void CheckStatsIndicators(CardCharacterInconstantModel characterInconstantModel, CardCharacterInconstantModel prevCardCharacterInconstantModel)
        {
            if (characterInconstantModel.CurrentEnergy !=
                prevCardCharacterInconstantModel.CurrentEnergy)
            {
                StartCoroutine(AnimateStatsIndicators(characterInconstantModel.CurrentEnergy,
                    prevCardCharacterInconstantModel.CurrentEnergy,
                    _energyIndicator));
            }
            if (characterInconstantModel.CurrentHealth !=
                prevCardCharacterInconstantModel.CurrentHealth)
            {
                StartCoroutine(AnimateStatsIndicators(characterInconstantModel.CurrentHealth,
                    prevCardCharacterInconstantModel.CurrentHealth,
                    _healthIndicator));
            }

            if(characterInconstantModel.CurrentHealth <= 0 && _characterMaterials.Length != 0)
            {
                _animateCharacterAppearance.Dissolve(_characterMaterials, _animationAmountPerSecond);
            }
        }

        IEnumerator AnimateStatsIndicators(int currentStats,
            int prevStats, TextMeshProUGUI textMesh)
        {
            for (int i = 0; i < 11; i++)
            {
                textMesh.text = ((int)Mathf.Lerp(prevStats, currentStats, i*0.1f)).ToString();
                yield return new WaitForSeconds(_statsIndicatorsAnimationSpeed);
            }
            ApplyIdleAnimation();
        }

        private void ApplyIdleAnimation()
        {
            _animateCharacterCollision?
                            .StartIdleAnimation(_animator);
        }
    }
}
