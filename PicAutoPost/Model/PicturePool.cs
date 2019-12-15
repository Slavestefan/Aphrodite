using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slavestefan.Aphrodite.Model
{
    public class PicturePool
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdPicturePool { get; set; }
        public IList<Picture> Pictures { get; set; }
        public User Owner { get; set; }
    }
}