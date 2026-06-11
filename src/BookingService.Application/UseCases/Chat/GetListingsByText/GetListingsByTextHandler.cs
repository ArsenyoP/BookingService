using Booking.Application.Abstractions;
using Booking.Application.DTOs.Chat;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using System.Text;
using static Booking.Application.DTOs.Chat.SearchMatchDtos;

namespace Booking.Application.UseCases.Chat.GetListingsByText
{
    public class GetListingsByTextHandler(IQdrantService _qdrantService,
        IChatService _chatService, IListingRepository _listingRepository) : IQueryHandler<GetListingsByTextQuery, ChatListingResponse>
    {
        public async Task<Result<ChatListingResponse>> Handle(GetListingsByTextQuery request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.userText))
                return Result<ChatListingResponse>.Failure(ChatErrors.EmptyMessage);

            var searchMatches = await _qdrantService.SearchAsync(
                userMessage: request.userText,
                city: request.city,
                maxPrice: null,
                limit: 4,
                CollectionName: "listings",
                ct);

            if (!searchMatches.Any())
            {
                string fallbackAnswer = await _chatService.GenerateRoomAiResponse(
                    userContext: request.userText,
                    systemMessage: $"Користувач шукає житло у місті {request.city}, але варіантів немає. Повідом про це.",
                    ct: ct
                );
                return Result<ChatListingResponse>.Success(new ChatListingResponse(fallbackAnswer, new()));
            }

            var listingIds = searchMatches.Select(x => x.Id).ToList();
            var listingsFromDb = await _listingRepository.GetByIds(listingIds, ct);

            var contextBuilder = new StringBuilder();
            var matchedListingDto = new List<ListingSearchMatchDto>();

            foreach (var listing in listingsFromDb)
            {
                string amenities = listing.Amenities.Any()
                    ? string.Join(", ", listing.Amenities.Select(a => a.Name))
                    : "не вказано";

                contextBuilder.AppendLine(@$"- ID: {listing.Id}, Назва: {listing.Title}Зручності: {amenities}.
                    Вулиця: {listing.Address.Street}.");

                double score = searchMatches.First(m => m.Id == listing.Id).Score;
                matchedListingDto.Add(new ListingSearchMatchDto(listing.Id, listing.Title, listing.AverageRating, score));
            }

            string systemPrompt = $@"Твоє завдання — допомогти користувачу підібрати варіант.
                        КРИТИЧНІ ПРАВИЛА ДЛЯ ВІДПОВІДІ:
                        1. НІКОЛИ не виводь і не згадуй технічні ідентифікатори (ID, Guid) у тексті відповіді користувачу. Ці дані є секретними та внутрішніми.
                        2. Спілкуйся природно, як живий менеджер підтримки. Формуй гарні списки без коду, технічних назв або UUID-ів.
                        3. Описуй лише назву, ціну, опис та зручності людською мовою.
                        4. Тобі потрібно не надто нав'язливо, але продавати бронювання, тобто без негативу, де це не необхідно.
                        5. Проте не треба про кожен готель розказувати сухі дані, тільки основне, так як повні дані є у об'єкті який йде разом з цією відповідю.
                        Ось список готелей, які найбільше підходять гостю, по них і орієнтуйся.
                {contextBuilder}";

            string aiAnswer = await _chatService.GenerateRoomAiResponse(
                userContext: request.userText,
                systemMessage: systemPrompt,
                ct: ct
            );

            return Result<ChatListingResponse>.Success(new ChatListingResponse(aiAnswer, matchedListingDto));
        }
    }
}
