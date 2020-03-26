

using System;
using System.ComponentModel.DataAnnotations;

namespace Slavestefan.Aphrodite.Model.Tasks
{
    public class TaskHistory
    {
        [Required]
        [Key]
        public Task Task { get; set; }
        [Required]
        [Key]
        public DateTime Time { get; set; }
        [Required]
        [Key]
        public User Picker { get; set; }
    }
}