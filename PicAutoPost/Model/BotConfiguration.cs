

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slavestefan.Aphrodite.Model
{
    public class BotConfiguration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdBotConfiguration { get; set; }
        
        public ulong ChannelId { get; set; }
        public string Key { get; set; }
        public string ValueString { get; set; }
        public int ValueInt { get; set; }
        public bool ValueBool { get; set; }
        public ulong ValueUlong { get; set; }
    }
}