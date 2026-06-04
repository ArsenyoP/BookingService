using Booking.Application.DTOs.Bookings;
using Booking.Application.UseCases.Bookings.CreateBooking;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace BookingService.IntegrationTests.IntegrationTests.BookingIntegrationTests
{
    public class CreateBookingIntegrationTest : BaseIntegrationTest
    {
        private readonly Helpers helpers;
        private readonly IntegrationTestWebAppFactory _factory;

        public CreateBookingIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory)
        {
            helpers = new Helpers(Sender, DbContext);
            _factory = factory;
        }

        [Fact]
        public async Task Create_ValidData_ReturnsSuccess()
        {
            var listingId = await helpers.CreateTestListing();

            var roomId = await helpers.CreateTestRoom(listingId);

            var userId = await helpers.CreateTestUser();

            var startDate = new DateOnly(2030, 12, 1);
            var endDate = new DateOnly(2030, 12, 20);

            var createBookingDto = new CreateBookingDto(roomId,
                startDate,
                endDate,
                1,
                2);

            var createBookingCommand = new CreateBookingCommand(createBookingDto, userId);


            var result = await Sender.Send(createBookingCommand);


            result.IsSuccess.Should().Be(true);

            var bookingInDb = await DbContext.Bookings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.RoomId == roomId && x.GuestId == userId);

            bookingInDb.Should().NotBeNull();
            bookingInDb!.Period.StartDate.Should().Be(startDate);
            bookingInDb.Period.EndDate.Should().Be(endDate);
        }

        [Fact]
        public async Task Create_DateCrossing_ReturnsFailure()
        {

            var listingId = await helpers.CreateTestListing("2");

            var roomId = await helpers.CreateTestRoom(listingId);

            var userId = await helpers.CreateTestUser();

            var firstBookingResult = await helpers.CreateTestBooking(roomId);



            var startDate = new DateOnly(2030, 12, 1);
            var endDate = new DateOnly(2030, 12, 20);

            var createBookingDto = new CreateBookingDto(roomId,
                startDate,
                endDate,
                1,
                2);

            var createBookingCommand = new CreateBookingCommand(createBookingDto, userId);


            var result = await Sender.Send(createBookingCommand);


            result.IsSuccess.Should().BeFalse();
        }

        //[Fact]
        //public async Task Create_ConcurrentRequestsForSameRoomAndDates_OnlyOneShouldSucceed()
        //{
        //    var listingId = await helpers.CreateTestListing("2");
        //    var roomId = await helpers.CreateTestRoom(listingId);
        //    var firstUserId = await helpers.CreateTestUser();

        //    var birthdayDate = new DateOnly(1999, 5, 20);
        //    var secondUserDto = new RegisterDto(
        //        "StasKovalUnique",
        //        "Stas",
        //        "Koval",
        //        "unique.stas.koval@gmail.com",
        //        "Password123!",
        //        birthdayDate);

        //    var createSecondUserCommand = new RegisterUserCommand(secondUserDto, "Admin");
        //    var secondUserResult = await Sender.Send(createSecondUserCommand);
        //    secondUserResult.IsSuccess.Should().BeTrue();

        //    var secondUserInDb = await DbContext.Users
        //        .FirstOrDefaultAsync(x => x.UserName == secondUserDto.UserName);

        //    secondUserInDb.Should().NotBeNull();
        //    var secondUserId = secondUserInDb!.Id;

        //    var startDate = new DateOnly(2031, 1, 10);
        //    var endDate = new DateOnly(2031, 1, 15);

        //    var firstBookingDto = new CreateBookingDto(roomId, startDate, endDate, 1, 0);
        //    var secondBookingDto = new CreateBookingDto(roomId, startDate, endDate, 1, 0);

        //    var firstCommand = new CreateBookingCommand(firstBookingDto, firstUserId);
        //    var secondCommand = new CreateBookingCommand(secondBookingDto, secondUserId);

        //    // 1. Створюємо два окремі ізольовані скоупи сервісів (як це робить ASP.NET Core для кожного запиту)
        //    using var scope1 = _factory.Services.CreateScope();
        //    using var scope2 = _factory.Services.CreateScope();

        //    // 2. Дістаємо окремі інстанси ISender для кожного потоку
        //    var sender1 = scope1.ServiceProvider.GetRequiredService<ISender>();
        //    var sender2 = scope2.ServiceProvider.GetRequiredService<ISender>();

        //    // 3. Стріляємо паралельно через різні сервіси
        //    var firstRequestTask = sender1.Send(firstCommand);
        //    var secondRequestTask = sender2.Send(secondCommand);

        //    var results = await Task.WhenAll(firstRequestTask, secondRequestTask);

        //    var successCount = results.Count(r => r.IsSuccess);
        //    var failureCount = results.Count(r => !r.IsSuccess);

        //    successCount.Should().Be(1);
        //    failureCount.Should().Be(1);

        //    var failedResult = results.First(r => !r.IsSuccess);
        //    failedResult.Error.Should().Be(BookingErrors.RoomNotAvaible);

        //    // 4. Перевіряємо фінальний стан через основний тестовий DbContext
        //    var bookingsInDbCount = await DbContext.Bookings
        //        .AsNoTracking()
        //        .CountAsync(x => x.RoomId == roomId && x.Period.StartDate == startDate);

        //    bookingsInDbCount.Should().Be(1);
        //}
    }
}
