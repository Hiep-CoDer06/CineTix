using CineTix.Dtos;
using CineTix.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineTix.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/showtimes")]
public sealed class ShowtimesApiController : ControllerBase
{
    private readonly IShowtimeBookingQueryService _showtimeBookingQueryService;

    public ShowtimesApiController(IShowtimeBookingQueryService showtimeBookingQueryService)
    {
        _showtimeBookingQueryService = showtimeBookingQueryService;
    }

    /// <summary>
    /// Lấy thông tin suất chiếu và các ghế còn trống để hiển thị ở màn hình đặt vé.
    /// </summary>
    [HttpGet("{showtimeId:int}/booking-data")]
    [ProducesResponseType(typeof(BookingShowtimeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingShowtimeResponse>> GetBookingData(
        int showtimeId,
        CancellationToken cancellationToken)
    {
        var response = await _showtimeBookingQueryService
            .GetBookingDataAsync(showtimeId, cancellationToken);

        return response == null ? NotFound() : Ok(response);
    }
}
