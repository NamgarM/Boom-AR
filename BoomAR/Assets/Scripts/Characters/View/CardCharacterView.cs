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
        private bool _isDissolved = true;

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
            // Animation
            if (_animateCharacterAppearance != null && !_isAnimationPlayed)
            {
                _isAnimationPlayed = _animateCharacterAppearance
                    .IsAppearingAnimationShowed(_characterMaterials, _animationAmountPerSecond);
            }
            if(_animateCharacterAppearance != null && _isDissolved == false)
            {
                _isDissolved = _animateCharacterAppearance
                    .IsDissolved(_characterMaterials, _animationAmountPerSecond);
                this.gameObject.SetActive(_isDissolved);
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
            CardCharacterInconstantModel prevCardCharacterInconstantModel,
            int opponentHealth)
        {
            if (opponent != this.gameObject && this.gameObject.activeInHierarchy)
            {
                // Play animation
                switch (animationType)
                {
                    case "Heal":
                        _soundManager?.Play("LongHealingProcessSFX", 1f);
                        _animateCharacterCollision?
                            .ShowHealingCollisionAnimation(opponent.transform.position, null, _healthIndicator
                            .transform.parent.gameObject, _character);
                        break;
                    case "Attack":
                        _soundManager?.Play("ShootingSFX", 1f);
                        this.transform
                            .LookAt(new Vector3(opponent.transform.position.x, this.transform.position.y, this.transform.position.z));
                        _animateCharacterCollision?
                            .ShowShootingAnimation(_animator);
                        break;
                }

                CheckStatsIndicators(characterInconstantModel, prevCardCharacterInconstantModel, opponentHealth);
            }
        }

        private void CheckStatsIndicators(CardCharacterInconstantModel characterInconstantModel, 
            CardCharacterInconstantModel prevCardCharacterInconstantModel, int opponentHealth)
        {
            if (characterInconstantModel.CurrentEnergy !=
                prevCardCharacterInconstantModel.CurrentEnergy)
            {
                StartCoroutine(AnimateStatsIndicators(characterInconstantModel.CurrentEnergy,
                    prevCardCharacterInconstantModel.CurrentEnergy,
                    _energyIndicator, false, characterInconstantModel, -1));
            }
            if (characterInconstantModel.CurrentHealth !=
                prevCardCharacterInconstantModel.CurrentHealth)
            {
                StartCoroutine(AnimateStatsIndicators(characterInconstantModel.CurrentHealth,
                    prevCardCharacterInconstantModel.CurrentHealth,
                    _healthIndicator, true, characterInconstantModel, opponentHealth));
            }
        }

        IEnumerator AnimateStatsIndicators(int currentStats,
            int prevStats, TextMeshProUGUI textMesh, bool isHealth, CardCharacterInconstantModel characterInconstantModel,
            int opponentHealth)
        {
            for (int i = 0; i < 11; i++)
            {
                textMesh.text = ((int)Mathf.Lerp(prevStats, currentStats, i*0.1f)).ToString();
                yield return new WaitForSeconds(_statsIndicatorsAnimationSpeed);
            }

            if (currentStats <= 0 && isHealth && _characterMaterials.Length != 0)
                _isDissolved = false;
            if (opponentHealth == 0 && isHealth)
                ApplyIdleAnimation();
        }

        private void ApplyIdleAnimation()
        {
            _animateCharacterCollision?
                            .StartIdleAnimation(_animator);
        }
    }
}
