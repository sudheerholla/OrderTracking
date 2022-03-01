using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using OrderTracking.Application.Interfaces;
using OrderTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderTracking.Infrastructure.Persistance
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        private readonly IDateTime dateTime;

        public ApplicationDbContext(DbContextOptions options,
                                    IDateTime dateTime) : base(options)
        {
            this.dateTime = dateTime;
        }

        public DbSet<Order> Orders { get; set; }

        public async Task BulkInsertAsync<T>(IList<T> entities, CancellationToken cancellationToken = default) where T : class
        {
            // cosmos db and inmemory does not allow bulk insert
            if (this.Database.IsCosmos() || this.Database.IsInMemory())
            {
                Set<T>().AddRange(entities);

                await SaveChangesAsync(cancellationToken);
            }
            else
            {
                await DbContextBulkExtensions.BulkInsertAsync<T>(this, entities);
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            if (this.Database.IsCosmos())
            {
                // CosmosDb specifics
                builder.Entity<Order>()
                    .OwnsMany(m => m.OrderItems);
                builder.Entity<Order>()
                   .OwnsOne(m => m.Customer);
               
                builder.Entity<Order>()
                    .ToContainer("Order").HasNoDiscriminator();

                builder.Entity<Order>()
                   .HasPartitionKey(o => o.RestaurantId);

            }

            base.OnModelCreating(builder);
        }
    }
}
