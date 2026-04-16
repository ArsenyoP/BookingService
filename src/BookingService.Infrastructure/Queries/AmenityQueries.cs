using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Entities;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Booking.Infrastructure.Queries
{
    public class AmenityQueries(string connectionString) : IAmenityQueries
    {
        public async Task<Amenity?> GetByNameAsync(string name, CancellationToken ct = default)
        {
            var connection = new SqlConnection(connectionString);

            const string sql = @"SELECT * FROM Amenities a 
                WHERE a.Name=@Name";

            var command = new CommandDefinition(
                sql,
                new { Name = name },
                cancellationToken: ct);

            var result = await connection.QueryFirstOrDefaultAsync<Amenity>(command);
            return result;
        }
    }
}
