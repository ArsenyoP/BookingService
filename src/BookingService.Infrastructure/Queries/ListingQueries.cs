using Booking.Application.DTOs.Amenities;
using Booking.Application.DTOs.Listings;
using Booking.Application.DTOs.Rooms;
using Booking.Application.Interfaces.IQueries;
using Booking.Domain.Entities;
using Dapper;
using Microsoft.Data.SqlClient;

namespace Booking.Infrastructure.Queries
{
    public class ListingQueries(string connectionString) : IListingQueries
    {
        public async Task<IReadOnlyList<ListingResponseDto>?> GetAllPagedAsync(int page, int pageSize,
            List<string>? amenityNames = null, CancellationToken ct = default)
        {
            var connection = new SqlConnection(connectionString);
            var offset = (page - 1) * pageSize;
            var namesCount = amenityNames?.Count ?? 0;
            var names = amenityNames ?? new List<string>();
            
            //Interview
            //TODO: Filters
            const string sql = """
                    SELECT 
                        l.Id, 
                        l.Title, 
                        l.Description, 
                        l.Address_Country AS Country, 
                        l.Address_City AS City, 
                        l.Address_Street AS Street, 
                        l.Address_HouseNumber AS HouseNumber, 
                        l.Address_Floor AS Floor, 
                        l.ListingType,
                        a.Id           AS AmenityId,
                        a.Name         AS Name,
                        a.Category     AS Category
                    FROM (
                        SELECT l2.Id FROM Listings l2
                        WHERE (@NamesCount = 0 OR (
                            SELECT COUNT(DISTINCT a2.Name)
                            FROM ListingAmenities la2
                            INNER JOIN Amenities a2 ON la2.AmenitiesId = a2.Id
                            WHERE la2.ListingId = l2.Id AND a2.Name IN @Names -- ВИПРАВЛЕНО: a2.Name замість l2.Name
                        ) = @NamesCount)
                        ORDER BY l2.Title
                        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY
                    ) AS paged
                    INNER JOIN Listings l     ON l.Id = paged.Id
                    LEFT JOIN  ListingAmenities la ON la.ListingId = l.Id
                    LEFT JOIN  Amenities a    ON a.Id = la.AmenitiesId
                    ORDER BY l.Title;
                    """;

            var listingDictionary = new Dictionary<Guid, ListingResponseDto>();

            await connection.QueryAsync<ListingResponseDto, AmenityDto?, ListingResponseDto>(
                sql,
                (listing, amenity) =>
                {
                    if (!listingDictionary.TryGetValue(listing.Id, out var existingListing))
                    {
                        existingListing = listing with { Amenities = new List<AmenityDto>() };
                        listingDictionary[listing.Id] = existingListing;
                    }

                    if (amenity is not null)
                        existingListing.Amenities.Add(amenity);

                    return existingListing;
                },
                new
                {
                    Offset = offset,
                    PageSize = pageSize,
                    Names = names,
                    NamesCount = namesCount
                },
                splitOn: "AmenityId"
            );

            return listingDictionary.Values.ToList();
        }

        public async Task<ListingResponseDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            using var connection = new SqlConnection(connectionString);

            const string sql = @"
                SELECT 
                    l.Id, 
                    l.Title, 
                    l.Description, 
                    l.Address_Country AS Country, 
                    l.Address_City AS City, 
                    l.Address_Street AS Street, 
                    l.Address_HouseNumber AS HouseNumber, 
                    l.Address_Floor AS Floor, 
                    ListingType,
                    a.Id           AS AmenityId,
                    a.Name         AS Name,
                    a.Category     AS Category
                FROM Listings l
                LEFT JOIN  ListingAmenities la ON la.ListingId = l.Id
                LEFT JOIN  Amenities a    ON a.Id = la.AmenitiesId
                WHERE l.Id = @Id";

            ListingResponseDto? result = null;

            await connection.QueryAsync<ListingResponseDto, AmenityDto?, ListingResponseDto>(
                sql,
                (listing, amenity) =>
                {
                    if (result == null)
                    {
                        result = listing;
                    }

                    if (amenity is not null && amenity.AmenityId != Guid.Empty)
                    {
                        result.Amenities.Add(amenity);
                    }
                    return listing;
                },
                new { Id = id },
                splitOn: "AmenityId"
                );

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
