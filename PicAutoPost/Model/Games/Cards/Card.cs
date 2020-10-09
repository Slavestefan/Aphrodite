

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slavestefan.Aphrodite.Model.Games.Cards
{
    public class Card
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdCard { get; set; }
        public string Value { get; set; }

        public override string ToString()
            => Value.Replace("S", "♠", StringComparison.OrdinalIgnoreCase)
                .Replace("H", "♥", StringComparison.OrdinalIgnoreCase)
                .Replace("D", "♦", StringComparison.OrdinalIgnoreCase)
                .Replace("C", "♣", StringComparison.OrdinalIgnoreCase);
    }
}