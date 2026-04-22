using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Entities;
using Microsoft.Data.SqlClient;
using Dapper;
using Booking.Application.DTOs.Amenities;
using System.Threading;

namespace Booking.Infrastructure.Queries
{
    public class AmenityQueries(string connectionString) : IAmenityQueries
    {
        public async Task<IReadOnlyList<AmenityDto>> GetAllAmenities(int page, int pageSize, CancellationToken ct = default)
        {
            var connection = new SqlConnection(connectionString);

            const string sql = @"SELECT 
                    a.Id AS AmenityId,
                    a.Name,
                    a.Category 
                    FROM Amenities a 
                ORDER BY a.Id
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var offset = (page - 1) * pageSize;

            var command = new CommandDefinition(
                 sql,
                 new { Offset = offset, PageSize = pageSize },
                 cancellationToken: ct);

            var result = await connection.QueryAsync<AmenityDto>(command);

            return result.ToList().AsReadOnly();
        }

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
