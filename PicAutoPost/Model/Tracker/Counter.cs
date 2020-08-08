

using System;
using System.ComponentModel.DataAnnotations;
using Slavestefan.Aphrodite.Model.Users;

namespace Slavestefan.Aphrodite.Model.Tracker
{
    public class Counter
    {
        [Key]
        public Guid IdCounter {get;set;}

        public int TotalAmount {get;set;}
        public int CompletedAmount {get;set;}
        public string Name {get;set;}
        public string Description {get;set;}
        public User User {get;set;}
        public bool IsHidden {get;set;}
    }
}