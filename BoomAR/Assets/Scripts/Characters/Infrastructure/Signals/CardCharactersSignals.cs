using GrowAR.Characters.Models;
using UnityEngine;

namespace GrowAR.Characters.Infrastructure
{
    public class CardCharactersSignals
    {
        public class CardCharacterCollidedSignal
        {
            public string CharacterId;
            public string CollidedCharacterId;
        }
    }
}
