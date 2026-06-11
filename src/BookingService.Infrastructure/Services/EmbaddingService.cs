using Booking.Domain.DomainEvents;
using Booking.Domain.Interfaces.Services;
using Google.Protobuf.Collections;
using Microsoft.Extensions.AI;
using Qdrant.Client;
using Qdrant.Client.Grpc;

namespace Booking.Infrastructure.Services
{
    public class EmbaddingService(IEmbeddingGenerator<string, Embedding<float>> _embeddingGenerator,
        IQdrantClient _qdrantClient) : IEmbaddingService
    {
        public async Task EmbaddeListingCreatedEvent(LisitngCreatedDomainEvent domainEvent, CancellationToken ct = default)
        {
            var options = new EmbeddingGenerationOptions
            {
                AdditionalProperties = new()
                {
                    { "outputDimensionality", 768 }
                }
            };
            var generatedEmbeddings = await _embeddingGenerator.GenerateAsync(
                    new[] { domainEvent.searchText },
                    options: options,
                    cancellationToken: ct);

            float[] vectorArray = generatedEmbeddings[0].Vector.ToArray();

            var payload = new MapField<string, Value>
            {
                { "city", new Value { StringValue = domainEvent.city.ToLower().Trim() } },
                { "street", new Value { StringValue = domainEvent.street.ToLower().Trim() } }
            };

            var point = new PointStruct
            {
                Id = new PointId { Uuid = domainEvent.listingId.ToString() },
                Vectors = vectorArray,
                Payload = { payload }
            };


            await _qdrantClient.UpsertAsync(
                collectionName: "listings",
                points: new[] { point },
                cancellationToken: ct
            );
        }

        public async Task EmbaddeRoomCreatedEvent(RoomCreatedDomainEvent @domainEvent, CancellationToken ct = default)
        {
            var options = new EmbeddingGenerationOptions
            {
                AdditionalProperties = new()
                {
                    { "outputDimensionality", 768 }
                }
            };
            var generatedEmbeddings = await _embeddingGenerator.GenerateAsync(
                    new[] { domainEvent.searchText },
                    options: options,
                    cancellationToken: ct);

            float[] vectorArray = generatedEmbeddings[0].Vector.ToArray();

            var payload = new MapField<string, Value>
            {
                { "city", new Value { StringValue = domainEvent.city.ToLower().Trim() } },
                { "price", new Value { DoubleValue = (double)domainEvent.pricePerNight } }
            };

            var point = new PointStruct
            {
                Id = new PointId { Uuid = domainEvent.roomId.ToString() },
                Vectors = vectorArray,
                Payload = { payload }
            };


            await _qdrantClient.UpsertAsync(
                collectionName: "rooms",
                points: new[] { point },
                cancellationToken: ct
            );
        }
    }
}
