using Microsoft.EntityFrameworkCore;

namespace Slavestefan.PicAutoPost.Model
{
    public class PicAutoPostContext : DbContext
    {
        public PicAutoPostContext() : base()
        {

        }

        public PicAutoPostContext(DbContextOptions<PicAutoPostContext> options) : base(options)
        {

        }

        public DbSet<PostConfiguration> Configurations { get; set; }
        public DbSet<Picture> Pictures { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Log> Logs { get; set; }
    }
} 