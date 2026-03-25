using Booking.Domain.Common;
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

        public Amenity(string name, string category)
        {
            Id = Guid.NewGuid();
            Name = name;
            Category = category;
        }
    }
}
