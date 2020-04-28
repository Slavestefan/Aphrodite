

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slavestefan.Aphrodite.Model.Alias
{
    public class ChannelAlias
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdAlias { get; set; }

        public ulong ChannelId { get; set; }
        public string Alias { get; set; }
    }
}