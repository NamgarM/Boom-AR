using GrowAR.Characters.Infrastructure;
using GrowAR.Characters.Models;
using GrowAR.Characters.View;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace GrowAR.Characters
{
    public class CardCharacterController
    {
        // Set starting health
        // Take damage
        // Make damage ?
        // Future: Use skill
        private CardCharacterView _characterView; //{ get; private set; }
        private SignalBus _signalBus;

        public Action<string, CardCharacterInconstantModel, GameObject, CardCharacterInconstantModel, int> CardsCollided;
        public Action<CardCharacterInconstantModel> RemoveCards;

        public CardCharacterConstantModel _characterConstantModel = new CardCharacterConstantModel();
        public CardCharacterInconstantModel _characterInconstantModel;
        public List<CardCharacterConstantModel> _characterConstantModels = new List<CardCharacterConstantModel>();
        public List<CardCharacterInconstantModel> _characterInconstantModels = new List<CardCharacterInconstantModel>();

        public Dictionary<string, CardCharacterModel> CardCharacterModels = new Dictionary<string, CardCharacterModel>();

        private string _characterId;
        private string _collidedCharacterId;

        public CardCharacterController(SignalBus signalBus)
        {
            _signalBus = signalBus;

            // To do: load from json [BA-24] Saving system
            CardCharacterConstantModel jinxModel = new CardCharacterConstantModel();
            jinxModel.CharacterId = "JinxCard";
            jinxModel.StartingEnergy = 5;
            jinxModel.StartingHealth = 2;
            jinxModel.Skill = "Attack";
            _characterConstantModels.Add(jinxModel);

            CardCharacterConstantModel healingModel = new CardCharacterConstantModel();
            healingModel.CharacterId = "HealingCard";
            healingModel.StartingEnergy = 0;
            healingModel.StartingHealth = 4;
            healingModel.Skill = "Heal";
            _characterConstantModels.Add(healingModel);

            CardCharacterConstantModel opponentModel = new CardCharacterConstantModel();
            opponentModel.CharacterId = "BlitzcrankCard";
            opponentModel.StartingEnergy = 2;
            opponentModel.StartingHealth = 1;
            opponentModel.Skill = "Attack";
            _characterConstantModels.Add(opponentModel);

        }

        public void ApplyCollision(CardCharactersSignals.CardCharacterCollidedSignal characterCollidedSignal)
        {
            if (_characterId != characterCollidedSignal.CollidedCharacterId)
            {
                // search models
                CardCharacterConstantModel currentCardCharacterConstantModel =
                    SearchConstantModel(characterCollidedSignal.CharacterId);
                CardCharacterConstantModel collidedCardCharacterConstantModel =
                    SearchConstantModel(characterCollidedSignal.CollidedCharacterId);

                CardCharacterInconstantModel currentCardCharacterInconstantModel =
                    SearchInconstantModel(characterCollidedSignal.CharacterId);
                CardCharacterInconstantModel collidedCardCharacterInconstantModel =
                    SearchInconstantModel(characterCollidedSignal.CollidedCharacterId);
                // For health indicators
                CardCharacterInconstantModel prevCardCharacterInconstantModel = new CardCharacterInconstantModel()
                {
                    CurrentEnergy =
                    currentCardCharacterInconstantModel.CurrentEnergy,
                    CurrentHealth =
                    currentCardCharacterInconstantModel.CurrentHealth
                };
                CardCharacterInconstantModel prevCollidedCardCharacterInconstantModel = new CardCharacterInconstantModel()
                {
                    CurrentEnergy =
                    collidedCardCharacterInconstantModel.CurrentEnergy,
                    CurrentHealth =
                    collidedCardCharacterInconstantModel.CurrentHealth
                };

                // Apply power up
                if (currentCardCharacterConstantModel.Skill == "Heal")
                {
                    AddNewStats(collidedCardCharacterInconstantModel, currentCardCharacterInconstantModel);
                    Nullify(currentCardCharacterInconstantModel);
                    RemoveCards.Invoke(currentCardCharacterInconstantModel);

                    // Update view
                    CardsCollided.Invoke(currentCardCharacterConstantModel.Skill, currentCardCharacterInconstantModel,
                        characterCollidedSignal.OpponentObject,
                        prevCardCharacterInconstantModel, -1);
                    CardsCollided.Invoke(null, collidedCardCharacterInconstantModel, null,
                        prevCollidedCardCharacterInconstantModel, -1);
                }
                else if (collidedCardCharacterConstantModel.Skill == "Heal")
                {
                    AddNewStats(currentCardCharacterInconstantModel, collidedCardCharacterInconstantModel);
                    Nullify(collidedCardCharacterInconstantModel);
                    RemoveCards.Invoke(collidedCardCharacterInconstantModel);

                    // Update view
                    CardsCollided.Invoke(collidedCardCharacterConstantModel.Skill, collidedCardCharacterInconstantModel,
                        characterCollidedSignal.OpponentObject,
                        prevCardCharacterInconstantModel, -1);
                    CardsCollided.Invoke(null, currentCardCharacterInconstantModel, null,
                        prevCollidedCardCharacterInconstantModel, -1);
                }
                else if (currentCardCharacterConstantModel.Skill == "Attack"
                  && collidedCardCharacterConstantModel.Skill == "Attack")
                {
                    ApplyNewStats(currentCardCharacterInconstantModel, collidedCardCharacterInconstantModel);
                    ApplyNewStats(collidedCardCharacterInconstantModel, prevCardCharacterInconstantModel);

                    // Update view
                    CardsCollided.Invoke(collidedCardCharacterConstantModel.Skill, collidedCardCharacterInconstantModel,
                        characterCollidedSignal.CardCharacterView.gameObject,
                        prevCardCharacterInconstantModel, currentCardCharacterInconstantModel.CurrentHealth);
                    CardsCollided.Invoke(currentCardCharacterConstantModel.Skill, currentCardCharacterInconstantModel, 
                        characterCollidedSignal.OpponentObject,
                        prevCollidedCardCharacterInconstantModel, collidedCardCharacterInconstantModel.CurrentHealth);
                }

                // Save in dictionary
                CardCharacterModels[characterCollidedSignal.CharacterId].CharacterInconstantModel =
                    currentCardCharacterInconstantModel;
                CardCharacterModels[characterCollidedSignal.CollidedCharacterId].CharacterInconstantModel =
                    collidedCardCharacterInconstantModel;

                // To clean next time
                _characterId = characterCollidedSignal.CharacterId;
                _collidedCharacterId = characterCollidedSignal.CollidedCharacterId;

            }
            else
            {
                _characterId = null;
                _collidedCharacterId = null;
            }
        }

        private void ApplyNewStats(CardCharacterInconstantModel cardInconstantModel, 
            CardCharacterInconstantModel applyingCardModel)
        {
            cardInconstantModel.CurrentHealth -= applyingCardModel.CurrentEnergy;
            cardInconstantModel.CurrentEnergy = 0; 

            if(cardInconstantModel.CurrentHealth <= 0)
            {
                cardInconstantModel.CurrentHealth = 0;
                RemoveCards.Invoke(cardInconstantModel);
            }
        }

        private void AddNewStats(CardCharacterInconstantModel cardInconstantModel,
            CardCharacterInconstantModel applyingCardModel)
        {
            cardInconstantModel.CurrentHealth += applyingCardModel.CurrentHealth;
            cardInconstantModel.CurrentEnergy += applyingCardModel.CurrentEnergy;
        }

        private void Nullify(CardCharacterInconstantModel applyingCardModel)
        {
            applyingCardModel.CurrentEnergy = 0;
            applyingCardModel.CurrentHealth = 0;
        }

        public CardCharacterInconstantModel SearchInconstantModel(string characterId)
        {
            return CardCharacterModels[characterId].CharacterInconstantModel;
        }

        public CardCharacterConstantModel SearchConstantModel(string characterId)
        {
            return CardCharacterModels[characterId].CharacterConstantModel;
        }

        public void SetCharacterView(CardCharacterView characterView)
        {
            _characterView = characterView;
        }
    }
}
