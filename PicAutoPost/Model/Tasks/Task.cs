

using System;
using System.ComponentModel.DataAnnotations;

namespace Slavestefan.Aphrodite.Model.Tasks
{
    public class Task
    {
        [Key]
        public Guid IdTask { get; set; }
        [Required]
        public User Owner { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public string HiddenDescription { get; set; }
    }
}