using Booking.Application.DTOs.Listings;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Booking.Infrastructure.Queries
{
    public class ListingQueries(string connectionString) : IListingQueries
    {
        public async Task<IReadOnlyList<ListingResponseDto>?> GetAllPagedAsync(int page, int pageSize, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);
            var offset = (page - 1) * pageSize;

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
                ORDER BY Title
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var command = new CommandDefinition(
                sql,
                new { Offset = offset, PageSize = pageSize },
                cancellationToken: ct);

            var result = await connection.QueryAsync<ListingResponseDto>(command);
            return result.ToList().AsReadOnly();
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

        public async Task<Listing?> GetEntityByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);
            const string sql = @"SELECT *
                FROM Listings r 
                WHERE r.Id=@Id";

            var command = new CommandDefinition(
                 sql,
                new { Id = id },
                cancellationToken: ct);

            var result = await connection.QueryFirstOrDefaultAsync<Listing>(command);

            return result;
        }
    }
}
