using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram_V2.Core.Models;
using Telegram_V2.Infrastructure.Database;
using Telegram_V2.Core.Enums;
using Telegram_V2.Core.Dtos;

[ApiController]
[Route("api/calls")]
public class CallsController : ControllerBase
{
    private readonly Context _context;

    public CallsController(Context context)
    {
        _context = context;
    }

    // ✅ Yangi qo'ng'iroqni boshlash
    [HttpPost("start")]
    public async Task<IActionResult> StartCall([FromBody] CallStartDto dto)
    {
        var call = new CallHistory
        {
            CallerId = dto.CallerId,
            RecieverId = dto.ReceiverId,
            CallType = dto.CallType,
            StartedAt = DateTime.UtcNow,
            CallStatus = CallStatus.Ringing // Endi ishlaydi
        };

        _context.CallHistories.Add(call);
        await _context.SaveChangesAsync();

        return Ok(new { CallId = call.Id });
    }

    // ✅ Qo'ng'iroqni tugatish
    [HttpPost("end/{callId}")]
    public async Task<IActionResult> EndCall(int callId, [FromBody] CallEndDto dto)
    {
        var call = await _context.CallHistories.FindAsync(callId);
        if (call == null) return NotFound("Qo'ng'iroq topilmadi");

        call.EndedAt = DateTime.UtcNow;
        call.Duration = dto.Duration;
        call.CallStatus = dto.CallStatus;

        await _context.SaveChangesAsync();
        return Ok("Qo'ng'iroq tugatildi");
    }

    // ✅ Foydalanuvchining qo'ng'iroqlar tarixi
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserCallHistory(int userId)
    {
        var calls = await _context.CallHistories
            .Where(c => c.CallerId == userId || c.RecieverId == userId)
            .Include(c => c.Caller)
            .Include(c => c.Receiver)
            .OrderByDescending(c => c.StartedAt)
            .Take(100)
            .Select(c => new
            {
                c.Id,
                Caller = new { c.Caller.Id, c.Caller.UserName },
                Receiver = new { c.Receiver.Id, c.Receiver.UserName },
                CallType = c.CallType.ToString(),
                Duration = c.Duration.ToString(),
                StartedAt = c.StartedAt,
                EndedAt = c.EndedAt,
                CallStatus = c.CallStatus.ToString()
            })
            .ToListAsync();

        return Ok(calls);
    }
}
