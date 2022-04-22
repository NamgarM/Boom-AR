using BoomAR.Characters;
using UnityEngine;

namespace BoomAR.Cards.Infrastructure
{
    public class CardSignals : MonoBehaviour
    {
        public class CardSpawnedSignal
        {
            public CharacterTypes CharacterType;
            public string SoundName;
            public float Delay;
        }
    }
}
