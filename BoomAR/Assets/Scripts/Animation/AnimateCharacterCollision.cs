using DG.Tweening;
using System;
using UnityEngine;

public class AnimateCharacterCollision : MonoBehaviour
{
    public void ShowCollisionAnimation(Vector3 to, string type, GameObject canvas)
    {
        canvas.SetActive(false);
        Sequence collisionAnimationSequence = DOTween.Sequence();
        collisionAnimationSequence
            .Append(Move(to, 5f));
        collisionAnimationSequence
            .Append(Scale(new Vector3(0.5f, 0.5f, 0.5f), 8f, 0, 0.1f));
        collisionAnimationSequence
            .Append(Scale(new Vector3(0.5f, 0.5f, 0.5f), 8f, 0, 0.1f));
        collisionAnimationSequence
            .AppendCallback(SwitchOff);
    }

    private void SwitchOff()
    {
        this.transform.parent.gameObject.SetActive(false);
    }

    private DG.Tweening.Tween Move(Vector3 to, float duration)
    {
        return transform.parent
            .DOMove(to, 5f);
    }

    private DG.Tweening.Tween Scale(Vector3 scaleTo, float duration, 
        int vibratio, float elasticity)
    {
        return transform
            .DOPunchScale(scaleTo, duration, vibratio, elasticity);
    }
}
