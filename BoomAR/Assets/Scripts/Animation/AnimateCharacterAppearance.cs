using UnityEngine;

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
}
