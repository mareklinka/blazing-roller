using System;
namespace BlazingRoller.Client.Models
{
    public class DiceThrowLogModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public int? Result { get; set; }
    }
}