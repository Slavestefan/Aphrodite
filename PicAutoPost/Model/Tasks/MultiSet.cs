

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Slavestefan.Aphrodite.Model.Users;

namespace Slavestefan.Aphrodite.Model.Tasks
{
    public class MultiSet
    {
        [Key]
        public Guid IdMultiSet { get; set; }
        public string Name { get; set; }
        public IList<MultiSetTaskSet> MultiSetTaskSets { get; set; }
        public User Owner { get; set; }
    }
}