

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Slavestefan.Aphrodite.Model.Users;

namespace Slavestefan.Aphrodite.Model.Tasks
{
    public class TaskConfiguration
    {
        [Key]
        public Guid IdTaskConfiguration { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public User Owner { get; set; }
        public User Recipient { get; set; } //If recipient is null, anyone may do it.
        public int StartingAmount { get; set; }
        [Required]
        public TaskAmount TaskAmount { get; set; }
        public IList<Task> Tasks { get; set; }
    }

    public enum TaskAmount
    {
        Limited,
        Unlimited
    }
}