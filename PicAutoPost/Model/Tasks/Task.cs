﻿

using System;
using System.ComponentModel.DataAnnotations;
using Slavestefan.Aphrodite.Model.Users;

namespace Slavestefan.Aphrodite.Model.Tasks
{
    public class Task
    {
        [Key]
        public Guid IdTask { get; set; }
        public Uri Image { get; set; }
        public string Description { get; set; }
    }
}