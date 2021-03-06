﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slavestefan.Aphrodite.Model.Users
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdUser { get; set; }
        public string Username { get; set; }
        public ulong DiscordId { get; set; }
        public UserStatus Status { get; set; }
    }

    public enum UserStatus
    {
        None,
        Admin,
        Blocked
    }
}