using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slavestefan.Aphrodite.Model
{
    public class Log
    {
        [Key] 
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdLog { get; set; }
        public string ChannelName { get; set; }
        public ulong ChannelId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Author { get; set; }
        public ulong AuthorId { get; set; }
        public string Message { get; set; }
    }
}