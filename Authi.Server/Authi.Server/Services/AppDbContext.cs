using Authi.Common.Extensions;
using Authi.Common.Services;
using Authi.Server.Extensions;
using Authi.Server.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;

namespace Authi.Server.Services
{
    [Service]
    internal interface IAppDbContext
    {
        TEntity? Find<TEntity>(Guid id) where TEntity : class;
        TEntity[] Find<TEntity>(Func<TEntity, bool> predicate) where TEntity : class;
        void Insert<TEntity>(TEntity entity) where TEntity : class;
        void Update<TEntity>(TEntity entity) where TEntity : class;
        void Delete<TEntity>(TEntity entity) where TEntity : class;
        void Delete<TEntity>(TEntity[] entities) where TEntity : class;
    }

    internal class AppDbContext : DbContext, IAppDbContext
    {
        public AppDbContext()
        {
            Database.EnsureCreated();
        }

        public TEntity? Find<TEntity>(Guid id) where TEntity : class
        {
            return Set<TEntity>().Find(id);
        }

        public TEntity[] Find<TEntity>(Func<TEntity, bool> predicate) where TEntity : class
        {
            return Set<TEntity>().Where(predicate).ToArray();
        }

        public void Insert<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().Add(entity);
            SaveChanges();
        }

        public new void Update<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().Update(entity);
            SaveChanges();
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().Remove(entity);
            SaveChanges();
        }

        public void Delete<TEntity>(TEntity[] entities) where TEntity : class
        {
            Set<TEntity>().RemoveRange(entities);
            SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var healthMonitor = ServiceProvider.Current.Get<IAppHealthMonitor>();
            /* OMITTED IN OSS BUILD */
            optionsBuilder
                .UseMySql(
                    connectionString: "",
                    serverVersion: new MySqlServerVersion(""))
                .OnError(healthMonitor.ReportEvent);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var keyPairConverter = new ValueConverter<KeyPair, string>(
                o => o.ToJson(),
                j => j.FromJson<KeyPair>()!);

            modelBuilder.Entity<Client>()
                .ToTable("client")
                .HasKey(c => c.ClientId);
            modelBuilder.Entity<Client>()
                .Property(c => c.KeyPair)
                .HasConversion(keyPairConverter)
                .HasColumnType("json");

            modelBuilder.Entity<Data>()
                .ToTable("data")
                .HasKey(c => c.DataId);

            modelBuilder.Entity<Sync>()
                .ToTable("sync")
                .HasKey(c => c.SyncId);
            modelBuilder.Entity<Sync>()
                .Property(c => c.OneTimeKeyPair)
                .HasConversion(keyPairConverter)
                .HasColumnType("json");
        }
    }
}
