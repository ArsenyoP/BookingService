using Booking.Domain.Common;

namespace Booking.Domain.Interfaces.IRepositories
{
    public interface IBaseRepository<T> where T : Entity
    {
        public void Add(T obj);
        public void Delete(T obj);
    }
}
