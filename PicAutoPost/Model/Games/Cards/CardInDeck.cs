

using System;
using System.ComponentModel.DataAnnotations;

namespace Slavestefan.Aphrodite.Model.Games.Cards
{
    public class CardInDeck
    {
        [Key]
        public Guid IdCardInDeck { get; set; }

        public Card Card { get; set; }

        public Deck Deck { get; set; }

        public int Order { get; set; }
    }
}