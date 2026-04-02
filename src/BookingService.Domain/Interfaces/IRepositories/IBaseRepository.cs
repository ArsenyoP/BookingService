using Booking.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Interfaces.IRepositories
{
    public interface IBaseRepository<T> where T : Entity
    {
        public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
        public Task<IReadOnlyList<T>> GetAllAsync(int page, int pageSize, CancellationToken ct = default);
        public void Add(T obj);
        public void Delete(T obj);
    }
}
