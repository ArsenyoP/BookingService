using Booking.Application.UseCases.Bookings.GetAllBookings;
using FluentAssertions;

namespace BookingService.IntegrationTests.IntegrationTests.BookingIntegrationTests
{
    public class GetAllBookingsIntegrationTests : BaseIntegrationTest
    {
        private readonly Helpers helpers;

        public GetAllBookingsIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory)
        {
            helpers = new Helpers(Sender, DbContext);
        }

        [Fact]
        public async Task GetAll_ValidQuery_ShouldReturnPagedAndSortedBookings()
        {
            await helpers.CreateBunchOfBookings();

            var firstResultCommand = new GetAllBookingsQuery(1, 2);
            var secondResultCommand = new GetAllBookingsQuery(2, 2);


            var firstResult = await Sender.Send(firstResultCommand);
            var secondResult = await Sender.Send(secondResultCommand);


            firstResult.IsSuccess.Should().BeTrue();
            secondResult.IsSuccess.Should().BeTrue();

            firstResult.Value!.Count.Should().Be(2);
            secondResult.Value!.Count.Should().Be(1);
        }



    }
}
