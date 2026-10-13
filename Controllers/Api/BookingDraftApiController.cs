using CineTix.Data;
using CineTix.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CineTix.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/booking-draft")]
public sealed class BookingDraftApiController : ControllerBase
{
    /// <summary>
    /// Lưu suất chiếu đang chọn vào Session của khách hàng.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public IActionResult Save(SaveBookingDraftRequest request)
    {
        HttpContext.Session.SetInt32(BookingDraftSession.ShowtimeIdKey, request.ShowtimeId);
        return NoContent();
    }
}
