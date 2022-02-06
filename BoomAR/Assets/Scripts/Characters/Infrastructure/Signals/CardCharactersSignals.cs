using GrowAR.Characters.View;
using UnityEngine;

namespace GrowAR.Characters.Infrastructure
{
    public class CardCharactersSignals
    {
        public class CardCharacterCollidedSignal
        {
            public string CharacterId;
            public string CollidedCharacterId;
            public GameObject OpponentObject;
            public CardCharacterView CardCharacterView;
        }
    }
}
