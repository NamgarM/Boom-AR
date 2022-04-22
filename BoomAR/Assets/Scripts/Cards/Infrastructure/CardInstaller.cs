using Zenject;

namespace BoomAR.Cards.Infrastructure
{
    public class CardInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindSignals();
        }

        private void BindSignals()
        {
            BindCardSpawnedSignal();
        }

        private void BindCardSpawnedSignal()
        {
            Container.DeclareSignal<CardSignals.CardSpawnedSignal>();
            /*
            Container.BindSignal<CardSignals.CardSpawnedSignal>()
                .ToMethod<CardCharacterController>(x => x.AnimateCharacterAppearance)
                .FromResolve();*/
        }
    }
}
