using System.Collections.Generic;

namespace BlazingRoller.Shared
{
    public class DiceThrowConfiguration
    {
        public int RandomSeed { get; set; }

        public int Offset { get; set; }

        public List<DieThrowConfiguration> Dice { get; set; }
    }
}