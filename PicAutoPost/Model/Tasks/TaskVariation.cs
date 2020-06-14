

using System;
using System.ComponentModel.DataAnnotations;

namespace Slavestefan.Aphrodite.Model.Tasks
{
    public class TaskVariation
    {
        [Key]
        public Guid IdTaskVariation { get; set; }
        public Task Task { get; set; }
        public string Description { get; set; }
    }
}