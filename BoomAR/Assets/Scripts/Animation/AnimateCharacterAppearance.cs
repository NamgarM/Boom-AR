using System;
using UnityEngine;

namespace GrowAR.Animation
{
    public class AnimateCharacterAppearance : MonoBehaviour
    {
        private const string _animationFactorName = "_Cutoff";

        public void Initialize(Material[] materials)
        {
            foreach (Material material in materials)
            {
                material.SetFloat("_Cutoff", 1);
            }
        }

        public bool IsAppearingAnimationShowed(Material[] materials, float amountPerSecond)
        {
            foreach (Material material in materials)
            {
                if (material?.GetFloat(_animationFactorName) > 0f)
                    material?
                       .SetFloat(_animationFactorName,
                       material.GetFloat(_animationFactorName) - (float)(amountPerSecond * Time.deltaTime));
                else
                    return true;
            }
            return false;
        }

        public void ResetDissolveAnimation(Material[] materials)
        {
            foreach (Material material in materials)
            {
                if (material?.GetFloat(_animationFactorName) < 1f)
                    material?
                       .SetFloat(_animationFactorName, 1f);
            }
        }
    }
}
