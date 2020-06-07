using System;

namespace BlazingRoller.Unity
{
    [Serializable]
    public class DiceThrowConfiguration
    {
        public string ThrowId;

        public int DiceSet;

        public bool ReturnFinalConfiguration;

        public int RandomSeed;

        public int Offset;

        public DieThrowConfiguration[] Dice;
    }
}


