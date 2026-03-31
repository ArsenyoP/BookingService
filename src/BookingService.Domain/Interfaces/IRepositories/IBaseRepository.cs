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
        public Task<T?> GetById(Guid id);
        public Task<List<T>> GetAll(int page, int pageSize);
        public void Add(T obj);
        public void Delete(T obj);
    }
}
