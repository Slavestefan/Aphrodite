

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slavestefan.Aphrodite.Model.Users
{
    public class UserAlias
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdUserAlias { get; set; }
        public User User { get; set; }
        public string Alias { get; set; }
    }
}