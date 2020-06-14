

using System;
using System.ComponentModel.DataAnnotations;

namespace Slavestefan.Aphrodite.Model.Tasks
{
    public class MultiSetTaskSet
    {
        [Key]
        public Guid IdMultiSetTaskSet { get; set; }
        public Guid IdMultiSet { get; set; }
        public MultiSet MultiSet { get; set; }
        public Guid IdTaskSet { get; set; }
        public TaskSet TaskSet { get; set; }
    }
}