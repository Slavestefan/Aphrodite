

using System;
using System.ComponentModel.DataAnnotations;
using Slavestefan.Aphrodite.Model.Users;

namespace Slavestefan.Aphrodite.Model.Tracker
{
    public class CounterHistory
    {
        [Key]
        public Guid IdCounterHistory {get;set;}
        [Required]
        public Counter Counter {get;set;}
        [Required]
        public int AmountChanged {get;set;}
        [Required]
        public ChangeType ChangeType { get; set; }
        [Required]
        public User ByUser {get;set;}
        [Required]
        public DateTime Timestamp {get;set;}
    }

    public enum ChangeType
    {
        Progress,
        Goal
    }
}