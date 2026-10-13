using System.ComponentModel.DataAnnotations;

namespace CineTix.Dtos;

/// <summary>
/// Suất chiếu được lưu tạm trong Session của khách hàng.
/// </summary>
public sealed record SaveBookingDraftRequest
{
    [Range(1, int.MaxValue)]
    public int ShowtimeId { get; init; }
}
