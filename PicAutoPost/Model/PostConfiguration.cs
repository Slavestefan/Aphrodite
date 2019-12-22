using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slavestefan.Aphrodite.Model
{
    public class PostConfiguration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdConfiguration { get; set; }
        public List<PicturePool> Pool { get; set; }
        public ulong ChannelId { get; set; }
        public int MinPostingIntervalInMinutes { get; set; }
        public int MaxPostingIntervalInMinutes { get; set; }
        public int MinPostPerInterval { get; set; }
        public int MaxPostPerInterval { get; set; }
        public DateTime? LastPost { get; set; }
        public bool IsRunning { get; set; }
        public bool IsInSithMode { get; set; }
        public ulong UserId { get; set; }
    }
}