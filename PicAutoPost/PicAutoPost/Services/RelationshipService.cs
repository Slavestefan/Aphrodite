

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Model.Users;

namespace Slavestefan.Aphrodite.Web.Services
{
    public class RelationshipService
    {
        private readonly PicAutoPostContext _context;

        public RelationshipService(PicAutoPostContext context)
        {
            _context = context;
        }

        public async Task AddRelationshipRequest(User owner, User slave, OwnerSlaveRelationshipTypes relationshipType, Status status)
        {
            var existingRelationship = await _context.OwnerSlaveRelationships.Include(x => x.Slave).FirstOrDefaultAsync(x => x.Slave.IdUser == slave.IdUser);

            if (existingRelationship != null)
            {
                throw new InvalidOperationException(slave.Username + " already has an owner or request pending");
            }

            var relationship = new OwnerSlaveRelationship
            {
                Owner = owner,
                Slave = slave,
                Type = relationshipType,
                Status = status
            };

            await _context.OwnerSlaveRelationships.AddAsync(relationship);
            await _context.SaveChangesAsync();
        }

        public async Task AddSlaveRequest(User owner, User slave, OwnerSlaveRelationshipTypes relationshipType)
            => await AddRelationshipRequest(owner, slave, relationshipType, Status.PendingSlaveConfirmation);

        public async Task AddOwnerRequest(User owner, User slave, OwnerSlaveRelationshipTypes relationshipType)
            => await AddRelationshipRequest(owner, slave, relationshipType, Status.PendingOwnerConfirmation);

        public async Task<OwnerSlaveRelationship> GetRelationship(ulong ownerSnowflake, ulong slaveSnowflake)
            => await _context.OwnerSlaveRelationships.Include(x => x.Slave).Include(x => x.Owner).FirstOrDefaultAsync(x => x.Owner.DiscordId == ownerSnowflake && x.Slave.DiscordId == slaveSnowflake);
    }
}