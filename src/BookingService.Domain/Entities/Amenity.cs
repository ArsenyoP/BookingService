using Booking.Domain.Common;
using Booking.Domain.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Domain.Entities
{
    public class Amenity : Entity
    {
        public string Name { get; private set; } = string.Empty;
        public string Category { get; private set; } = string.Empty;

        private Amenity() { }

        private Amenity(string name, string category)
        {
            Id = Guid.NewGuid();
            Name = name;
            Category = category;
        }

        public static Result<Amenity> Create(string name, string category)
        {
            if (string.IsNullOrWhiteSpace(name)) return Result<Amenity>.Failure(AmenityErrors.EmptyName);
            return Result<Amenity>.Success(new Amenity(name, category));
        }
    }
}
