using System;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace BlazingRoller.Shared
{
    public class DiceThrowConfiguration
    {
        public Guid ThrowId { get; set; }

        public int DiceSet { get; set; }

        public bool ReturnFinalConfiguration { get; set; }

        public int RandomSeed { get; set; }

        public int Offset { get; set; }

        public List<DieThrowConfiguration> Dice { get; set; }

        public override string ToString()
        {
            if (Dice == null || Dice.Count == 0)
            {
                return string.Empty;
            }

            var groups = Dice.GroupBy(_ => new { _.Sides, _.Multiplier }).ToList();

            var sb = new StringBuilder();

            for (var i = 0; i < groups.Count; i++)
            {
                var group = groups[i];

                if (i == 0)
                {
                    sb.Append(group.Key.Multiplier * group.Count()).Append('D').Append(group.Key.Sides);
                }
                else
                {
                    if (group.Key.Multiplier > 0)
                    {
                        sb.Append(" + ")
                          .Append(group.Key.Multiplier * group.Count())
                          .Append('D')
                          .Append(group.Key.Sides);
                    }
                    else
                    {
                        sb.Append(" - ")
                          .Append(-group.Key.Multiplier * group.Count())
                          .Append('D')
                          .Append(group.Key.Sides);
                    }
                }
            }

            if (Offset > 0)
            {
                sb.Append(" + ").Append(Offset);
            }
            else if (Offset < 0)
            {
                sb.Append(" - ").Append(-Offset);
            }

            return sb.ToString();
        }
    }
}
