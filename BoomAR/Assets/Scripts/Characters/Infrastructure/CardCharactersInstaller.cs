using GrowAR.Characters.Models;
using GrowAR.Characters.View;
using UnityEngine;
using Zenject;

namespace GrowAR.Characters.Infrastructure
{
    public class CardCharactersInstaller : MonoInstaller // to do - make it installer and then make primary one installer
    {
        public GameObject JinxCardCharacterViewPrefab;
        public GameObject HealingCardCharacterViewPrefab;
        [SerializeField] private GameObject _cardsCollection;
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            BindModels();

            BindSignals();

            //BindView();

            BindController();
        }

        private void BindController()
        {
            CardCharacterController characterController = Container
                .Instantiate<CardCharacterController>();

            Container
                .Bind<CardCharacterController>()
                .FromInstance(characterController)
                .AsSingle();
        }

        private void BindSignals()
        {
            DeclareCharacterCollidedSignal();
        }

        private void DeclareCharacterCollidedSignal()
        {
            Container.DeclareSignal<CardCharactersSignals.CardCharacterCollidedSignal>();
            Container.BindSignal<CardCharactersSignals.CardCharacterCollidedSignal>()
                .ToMethod<CardCharacterController>(x => x.ApplyCollision).FromResolve();
        }

        private void BindModels()
        {
            CardCharacterConstantModel characterConstantModel = Container
                .Instantiate<CardCharacterConstantModel>();

            Container
            .Bind<CardCharacterConstantModel>()
            .FromInstance(characterConstantModel)
            .AsSingle();

            CardCharacterInconstantModel characterInconstantModel = Container
                .Instantiate<CardCharacterInconstantModel>();

            Container
                .Bind<CardCharacterInconstantModel>()
                .FromInstance(characterInconstantModel)
                .AsSingle();
        }

        private void BindView()
        {
            CardCharacterView jinxCardcharacterView = Container
                .InstantiatePrefabForComponent<CardCharacterView>(JinxCardCharacterViewPrefab, Vector3.zero,
                Quaternion.identity, _cardsCollection.transform);

            Container
            .Bind<CardCharacterView>()
            .FromInstance(jinxCardcharacterView)
            .AsSingle();
        }
    }
}
