using DG.Tweening;
using UnityEngine;

namespace GrowAR.Animation
{
    public class AnimateCharacterCollision
    {
        private Transform _characterTransform;
        public void ShowHealingCollisionAnimation(Vector3 to, string type, GameObject canvas, Transform characterTransform)
        {
            _characterTransform = characterTransform;
            canvas.SetActive(false);
            if (_characterTransform != null)
            {
                Sequence collisionAnimationSequence = DOTween.Sequence();
                collisionAnimationSequence
                    .Append(Move(to, 5f));
                collisionAnimationSequence
                    .Append(Scale(new Vector3(0.5f, 0.5f, 0.5f), 4f, 0, 0.1f));
                collisionAnimationSequence
                    .Append(Scale(new Vector3(0.5f, 0.5f, 0.5f), 4f, 0, 0.1f));
                collisionAnimationSequence
                    .AppendCallback(SwitchOff);
            }
        }

        private void SwitchOff()
        {
            _characterTransform?.parent.gameObject.SetActive(false);
        }

        private DG.Tweening.Tween Move(Vector3 to, float duration)
        {
            return _characterTransform?.parent
                .DOMove(to, 4f);
        }

        private DG.Tweening.Tween Scale(Vector3 scaleTo, float duration,
            int vibratio, float elasticity)
        {
            return _characterTransform?
                .DOPunchScale(scaleTo, duration, vibratio, elasticity);
        }

        internal void ShowShootingAnimation(Animator animator)
        {
            int ShootHash = Animator.StringToHash("DoShoot"); // to do: in constants
            if (animator != null)
                animator?.SetTrigger(ShootHash);
        }

        internal void StartIdleAnimation(Animator animator)
        {
            int IdleHash = Animator.StringToHash("DoIdle"); // to do: in constants
            if (animator != null)
                animator?.SetTrigger(IdleHash);
        }
    }
}
