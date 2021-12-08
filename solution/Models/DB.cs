using Microsoft.EntityFrameworkCore;

namespace Models
{
    internal class DB { }

    public class DbContentCore : DbContext
    {
        public DbSet<Entity.ConfigEntity> ConfigEntity { get; set; }
        public DbSet<Entity.DepartmentExtraEntity> DepartmentExtraEntity { get; set; }
        public DbSet<Entity.DepartmentEntity> DepartmentEntity { get; set; }
        public DbSet<Entity.DirExtraEntity> DirExtraEntity { get; set; }
        public DbSet<Entity.DirEntity> DirEntity { get; set; }
        public DbSet<Entity.FileExtraEntity> FileExtraEntity { get; set; }
        public DbSet<Entity.FileEntity> FileEntity { get; set; }
        public DbSet<Entity.MessageEntity> MessageEntity { get; set; }
        public DbSet<Entity.OuterTokenEntity> OuterTokenEntity { get; set; }
        public DbSet<Entity.TokenEntity> TokenEntity { get; set; }
        public DbSet<Entity.UserExtraEntity> UserExtraEntity { get; set; }
        public DbSet<Entity.UserEntity> UserEntity { get; set; }
        public DbSet<Entity.DepartmentFileEntity> DepartmentFileEntity { get; set; }
        public DbSet<Entity.TagEntity> TagEntity { get; set; }
        public DbSet<Entity.FileTagEntity> FileTagEntity { get; set; }

        public DbContentCore(DbContextOptions<DbContentCore> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder) { base.OnModelCreating(modelBuilder); }
    }

}
