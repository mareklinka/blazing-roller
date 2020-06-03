using System;

namespace BlazingRoller.Unity
{
    [Serializable]
    public class DiceThrowConfiguration
    {
        public int RandomSeed;

        public int Offset;

        public DieThrowConfiguration[] Dice;
    }
}


