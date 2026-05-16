namespace Booking.Application.DTOs.Reviews
{
    public sealed record ReviewResponseDto
    {
        public int Score { get; init; }
        public string Text { get; init; } = string.Empty;
        public string UserName { get; init; } = string.Empty;
        public string TargetType { get; init; } = string.Empty;
        public string TargetTitle { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public bool IsEdited { get; init; }
    }
}
