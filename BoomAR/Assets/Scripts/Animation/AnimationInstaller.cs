using GrowAR.Animation;
using Zenject;

namespace BoomAR.Animation
{
    public class AnimationInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            AnimateCharacterCollision animateCharacterCollision = Container
                    .Instantiate<AnimateCharacterCollision>();

            Container
                .Bind<AnimateCharacterCollision>()
                .FromInstance(animateCharacterCollision)
                .AsSingle();
        }
    }
}