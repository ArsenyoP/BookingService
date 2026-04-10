using Booking.Application.DTOs.Listings;
using Booking.Application.DTOs.Rooms;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booking.Infrastructure.Queries
{
    public class ListingQueries(string connectionString) : IListingQueries
    {
        public Task<IReadOnlyList<ListingResponseDto>?> GetAllPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ListingResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);

            const string sql = @"
                SELECT 
                    Id, 
                    Title, 
                    Description, 
                    Address_Country AS Country, 
                    Address_City AS City, 
                    Address_Street AS Street, 
                    Address_HouseNumber AS HouseNumber, 
                    Address_Floor AS Floor, 
                    ListingType 
                FROM Listings 
                WHERE Id = @Id";

            var command = new CommandDefinition(
                sql,
                new { Id = id },
                cancellationToken: ct);

            var result = await connection.QueryFirstOrDefaultAsync<ListingResponseDto>(command);
            return result;
        }

        public Task<Listing?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}
