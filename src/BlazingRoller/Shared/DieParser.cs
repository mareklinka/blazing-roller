using System.Text;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BlazingRoller.Shared
{
    public static class DieParser
    {
        public static ParsingResult Parse(string expression)
        {
            var dieRegex = new Regex("^(?<multi>[0-9]*)[dD](?<size>4|6|8|10|12|20)$");
            var numberRegex = new Regex("^[0-9]+$");

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var parts = SplitExpression(expression);

            if (parts.Length == 0)
            {
                return new ParsingResult(new List<DieDefinition>(), new List<int>());
            }

            var dice = new List<DieDefinition>();
            var constants = new List<int>();
            var currentModifier = '+';
            var dieId = 0;

            foreach (var part in parts)
            {
                var dieMatch = dieRegex.Match(part);
                var numberMatch = numberRegex.Match(part);

                if (dieMatch.Success)
                {
                    if (!int.TryParse(dieMatch.Groups["size"].Value, out var faces))
                    {
                        throw new Exception();
                    }

                    if (!Enum.IsDefined(typeof(Die), faces))
                    {
                        throw new Exception();
                    }

                    if (!int.TryParse(dieMatch.Groups["multi"].Value, out var multiplicity))
                    {
                        multiplicity = 1;
                    }

                    for (var i = 0; i < multiplicity; ++i)
                    {
                        dice.Add(new DieDefinition(dieId++, (Die)faces, currentModifier == '-' ? -1 : 1));
                    }

                    currentModifier = '+'; // modifiers are only valid until the next non-modifier
                }
                else if (numberMatch.Success)
                {
                    var n = int.Parse(part);
                    constants.Add(currentModifier == '-' ? -n : n);
                    currentModifier = '+'; // modifiers are only valid until the next non-modifier
                }
                else if (part == "+")
                {
                    currentModifier = '+';
                }
                else if (part == "-")
                {
                    currentModifier = '-';
                }
                else
                {
                    throw new Exception();
                }
            }

            return new ParsingResult(dice, constants);
        }

        private static string[] SplitExpression(string expression)
        {
            var parts = new List<string>();
            var sb = new StringBuilder();

            foreach (var c in expression)
            {
                if (char.IsWhiteSpace(c))
                {
                    if (sb.Length > 0)
                    {
                        parts.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                else if (c == '+' || c == '-')
                {
                    if (sb.Length > 0)
                    {
                        parts.Add(sb.ToString());
                        sb.Clear();
                    }

                    parts.Add(c.ToString());
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (sb.Length > 0)
            {
                parts.Add(sb.ToString());
            }

            return parts.ToArray();
        }

        public class ParsingResult
        {
            public ParsingResult(IReadOnlyCollection<DieDefinition> dice, IReadOnlyCollection<int> constants)
            {
                Dice = dice;
                Constants = constants;
            }

            public IReadOnlyCollection<DieDefinition> Dice { get; }

            public IReadOnlyCollection<int> Constants { get; }
        }

        public class DieDefinition
        {
            public DieDefinition(int id, Die die, int multiplicity)
            {
                Die = die;
                Multiplicity = multiplicity;
                Id = id;
            }

            public Die Die { get; }

            public int Multiplicity { get; }

            public int Id { get; }

            public override string ToString()
            {
                return $"{Id}: {Multiplicity}D{(int)Die}";
            }
        }
    }
}