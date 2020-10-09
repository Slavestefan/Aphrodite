

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Slavestefan.Aphrodite.Model.Games.Cards
{
    public class Deck
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdDeck { get; set; }

        public Stack<CardInDeck> Cards {get;set;}

        public Card Pop( )
        {
            return Cards.Pop().Card;
        }

        public Card Draw() => Pop();
    }
}