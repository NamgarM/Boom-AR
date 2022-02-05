using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GrowAR.Characters.Models
{
    [Serializable]
    public class CardCharacterConstantModel
    {
        public string CharacterId;
        //name
        public int StartingHealth; //{ get; private set; } // To do: put in json
        public int StartingEnergy;
        // To do: skill
        public string Skill;
        // Future type

        private void SetValues()
        {
            StartingHealth = 5;
            StartingEnergy = 2;
        }
    }
}
