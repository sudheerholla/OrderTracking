using Microsoft.EntityFrameworkCore;
using OrderTracking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderTracking.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Order> Orders { get; set; }

        Task BulkInsertAsync<T>(IList<T> entities, CancellationToken cancellationToken = default) where T : class;

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
