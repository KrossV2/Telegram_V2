using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Telegram_V2.Core.Models;
using Telegram_V2.Infrastructure.Database;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly Context _context;
    private readonly IWebHostEnvironment _environment;

    public FilesController(Context context, IWebHostEnvironment environment)
    {
        _context = context;
        _environment = environment;
    }

    // ✅ Chatga fayl yuborish
    [HttpPost("{chatId}/upload")]
    public async Task<IActionResult> UploadFile(int chatId, IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("Fayl yo'q");

        // Avval message yaratish (fayl xabari)
        var message = new Message
        {
            ChatId = chatId,
            SenderId = GetCurrentUserId(), // User ID ni olish uchun metod
            CreatedAt = DateTime.UtcNow
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        // Fayl ni saqlash
        var fileName = $"file_{message.Id}_{DateTime.UtcNow.Ticks}{Path.GetExtension(file.FileName)}";
        var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // FileAttachment yaratish
        var attachment = new FileAttachment
        {
            MessageId = message.Id,
            FileUrl = $"/uploads/{fileName}",
            FileType = file.ContentType,
            FileSize = file.Length,
            UploadedAt = DateTime.UtcNow
        };

        _context.FileAttachments.Add(attachment);

        // Message ni yangilash (file URL bilan)
        message.FileUrl = attachment.FileUrl;
        await _context.SaveChangesAsync();

        return Ok(new
        {
            FileId = attachment.Id,
            FileUrl = attachment.FileUrl,
            MessageId = message.Id
        });
    }

    // ✅ Chatdagi barcha fayllar
    [HttpGet("{chatId}")]
    public async Task<IActionResult> GetChatFiles(int chatId)
    {
        var files = await _context.FileAttachments
            .Where(f => f.Message.ChatId == chatId)
            .Include(f => f.Message)
            .ThenInclude(m => m.Sender)
            .Select(f => new
            {
                f.Id,
                f.FileUrl,
                f.FileType,
                f.FileSize,
                f.UploadedAt,
                UploadedBy = new { f.Message.Sender.Id, f.Message.Sender.UserName }
            })
            .ToListAsync();

        return Ok(files);
    }

    // ✅ Bitta faylni olish
    [HttpGet("file/{fileId}")]
    public async Task<IActionResult> GetFile(int fileId)
    {
        var file = await _context.FileAttachments
            .Include(f => f.Message)
            .FirstOrDefaultAsync(f => f.Id == fileId);

        if (file == null) return NotFound("Fayl topilmadi");

        return Ok(new
        {
            file.Id,
            file.FileUrl,
            file.FileType,
            file.FileSize,
            file.UploadedAt,
            MessageId = file.MessageId
        });
    }

    // ✅ Faylni o'chirish
    [HttpDelete("{fileId}")]
    public async Task<IActionResult> DeleteFile(int fileId)
    {
        var file = await _context.FileAttachments
            .Include(f => f.Message)
            .FirstOrDefaultAsync(f => f.Id == fileId);

        if (file == null) return NotFound("Fayl topilmadi");

        // Haqiqiy faylni o'chirish
        var filePath = Path.Combine(_environment.WebRootPath, "uploads", Path.GetFileName(file.FileUrl));
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        // Message ni ham o'chirish (agar kerak bo'lsa)
        _context.FileAttachments.Remove(file);
        await _context.SaveChangesAsync();

        return Ok("Fayl o'chirildi");
    }

    private int GetCurrentUserId()
    {
        // Soddalashtirilgan: Haqiqiy loyihada JWT token dan olish kerak
        return 1; // Default user ID
    }
}