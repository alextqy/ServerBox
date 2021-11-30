using Microsoft.EntityFrameworkCore;

namespace Models
{
    internal class DB { }

    public class DbContentCore : DbContext
    {
        public DbContentCore(DbContextOptions<DbContentCore> options) : base(options) { }
    }

}
