

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Slavestefan.Aphrodite.Model.Users
{
    public class OwnerSlaveRelationship
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid IdOwnerSlaveRelationship { get; set; }

        public User Owner { get; set; }
        public User Slave { get; set; }

        public OwnerSlaveRelationshipTypes Type { get; set; }
        public Status Status { get; set; }
    }

    [Flags]
    public enum OwnerSlaveRelationshipTypes
    {
        None = 0,
        Keyholder = 1 << 1,
        Public = 1 << 2, 
        Taskmaster = 1 << 3,
        Tracker = 1 << 4,
        FullControl = int.MaxValue
    }

    public enum Status
    {
        None,
        PendingOwnerConfirmation,
        PendingSlaveConfirmation,
        Confirmed,
        ReleaseRequested
    }
}