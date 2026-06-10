using Booking.Application.Abstractions;
using Booking.Application.DTOs.Chat;
using Booking.Application.Interfaces.Services;
using Booking.Domain.Common;
using Booking.Domain.Errors;
using Booking.Domain.Interfaces.IRepositories;
using Booking.Domain.Interfaces.Services;
using System.Text;

namespace Booking.Application.UseCases.Chat.GetByText
{
    public record RoomSearchMatchDto(Guid Id, string Title, decimal Price, double Confidence);

    public class GetByTextHandler(IChatService _chatService,
        IEmbaddingService _embaddingService, IQdrantService _qdrantService,
        IRoomRepository _roomRepository) : IQueryHandler<GetByTextQuery, ChatRoomResponse>
    {
        public async Task<Result<ChatRoomResponse>> Handle(GetByTextQuery request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.searchText))
                return Result<ChatRoomResponse>.Failure(ChatErrors.EmptyMessage);

            var searchMatches = await _qdrantService.SearchAsync(
                request.searchText,
                request.city,
                request.maxPrice,
                limit: 4,
                ct);

            if (!searchMatches.Any())
            {
                string fallbackAnswer = await _chatService.GenerateRoomAiResponse(
                    userContext: request.searchText,
                    systemMessage: $"Користувач шукає житло у місті {request.city}, але варіантів немає. Повідом про це.",
                    ct: ct
                );
                return Result<ChatRoomResponse>.Success(new ChatRoomResponse(fallbackAnswer, new()));
            }



            var roomIds = searchMatches.Select(m => m.RoomId).ToList();
            var roomsFromDb = await _roomRepository.GetByIds(roomIds, ct);

            var contextBuilder = new StringBuilder();
            var matchedRoomsDto = new List<RoomSearchMatchDto>();

            foreach (var room in roomsFromDb)
            {
                string amenities = room.Amenities.Any()
                    ? string.Join(", ", room.Amenities.Select(a => a.Name))
                    : "не вказано";

                contextBuilder.AppendLine(@$"- ID: {room.Id}, Назва: {room.Title}, Ціна: {room.PricePerNight} грн/ніч. Зручності: {amenities}.
                    Кількість місць для дорослих: {room.AdultsCapacity}. Кількість місць для дітей: {room.ChildrenCapacity} ");

                double score = searchMatches.First(m => m.RoomId == room.Id).Score;
                matchedRoomsDto.Add(new RoomSearchMatchDto(room.Id, room.Title, room.PricePerNight, score));
            }

            string systemPrompt = $@"Твоє завдання — допомогти користувачу підібрати варіант.
                        КРИТИЧНІ ПРАВИЛА ДЛЯ ВІДПОВІДІ:
                        1. НІКОЛИ не виводь і не згадуй технічні ідентифікатори (ID, Guid) у тексті відповіді користувачу. Ці дані є секретними та внутрішніми.
                        2. Спілкуйся природно, як живий менеджер підтримки. Формуй гарні списки без коду, технічних назв або UUID-ів.
                        3. Описуй лише назву, ціну, опис та зручності людською мовою.
                        4. Тобі потрібно не надто нав'язливо, але продавати бронювання, тобто без негативу, де це не необхідно.
                        5. Проте не треба про кожну кімнату розказувати сухі дані, тільки основне, так як повні дані є у об'єкті який йде разом з цією відповідю.
                        Ось список кімнат, які найбільше підходять гостю, по них і орієнтуйся.
                {contextBuilder}";

            string aiAnswer = await _chatService.GenerateRoomAiResponse(
                userContext: request.searchText,
                systemMessage: systemPrompt,
                ct: ct
            );

            return Result<ChatRoomResponse>.Success(new ChatRoomResponse(aiAnswer, matchedRoomsDto));

            throw new NotImplementedException();
        }
    }
}
