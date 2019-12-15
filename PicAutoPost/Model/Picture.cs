using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slavestefan.PicAutoPost.Model
{
    public class Picture
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdPicture { get; set; }
        public Uri Location { get; set; }
        public string Name { get; set; }
        public byte[] Raw { get; set; }
        public User User { get; set; }
    }
}
