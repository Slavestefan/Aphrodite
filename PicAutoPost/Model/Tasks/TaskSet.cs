

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Slavestefan.Aphrodite.Model.Users;

namespace Slavestefan.Aphrodite.Model.Tasks
{
    public class TaskSet
    {
        [Key]
        public Guid IdTaskSet { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public User Owner { get; set; }
        public User Recipient { get; set; } //If recipient is null, anyone may do it.
        public IList<Task> Tasks { get; set; }
        public bool DoAllowMultiroll { get; set; }
        public bool DoesMultirollRepeat { get; set; }
        public IList<MultiSetTaskSet> MultiSetTaskSets { get; set; }
    }
}