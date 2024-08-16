using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebApplication2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SmsController : ControllerBase
{
    private readonly NotificationDbContext _notificationDbContext;
    public SmsController(NotificationDbContext notificationDbContext)
    {
        _notificationDbContext = notificationDbContext;
    }

    [HttpPost]
    public async Task<SmsResponse> SendSmsAsync([FromBody] SmsRequest request)
    {
        if (request.SmsResponseId.HasValue)
        {
            return await ProcessSmsResponseReplyAsync(request);
        }
        // Check database to see if there is any conflict with Actions on the same phone number
        var existingSms = _notificationDbContext.SmsNotifications.Where(x => x.PhoneNumber == request.PhoneNumber && x.Status == SmsNotificationStatus.Active)
                                                .Include(x => x.Actions);

        // In reality there would probably be checking here based on the notification pool.
        // So if one service uses it's own pool of numbers, we could have duplicates outstanding
        // but we can't have duplicates within the same pool
        if (existingSms.Any())
        {
            var existingActions = existingSms.SelectMany(x => x.Actions).Select(x => x.ActionText).ToList();
            if (request.Actions != null && request.Actions.Any(x => existingActions.Contains(x)))
            {
                throw new Exception($"Action already exists for this phone number");
            }
        }

        var smsNotification = new SmsNotification
        {
            Id = Guid.NewGuid(),
            PhoneNumber = request.PhoneNumber,
            Message = request.Message,
            Status = SmsNotificationStatus.Active,
            Metadata = request.Metadata,
            Actions = request.Actions?.Select(x => new SmsNotificationAction
            {
                Id = Guid.NewGuid(),
                ActionText = x
            }).ToList()
        };
        _notificationDbContext.SmsNotifications.Add(smsNotification);
        await _notificationDbContext.SaveChangesAsync();
        return new SmsResponse
        {
            Id = smsNotification.Id,
            Message = "SMS sent successfully"
        };
    }

    private async Task<SmsResponse> ProcessSmsResponseReplyAsync(SmsRequest request)
    {
        // We likely also need a way to mark the original notification as completed, or expired
        // Sometimes one simple response is not enough, sometimes it is.
        var smsResponse = await _notificationDbContext.SmsResponses.Include(x => x.SmsNotification).Include(x => x.SmsNotificationAction).FirstOrDefaultAsync(x => x.Id == request.SmsResponseId);
        if (smsResponse == null)
        {
            throw new Exception("Sms response not found");
        }

        // What we'd actually do here is send a new twilio message to the user
        // But we'd explicitly send it from the phone number that the user replied to
        // This way the thread is consistent.

        return new SmsResponse
        {
            Id = smsResponse.Id,
            Message = "SMS response processed successfully"
        };
    }
}

public class SmsResponse
{
    public Guid Id { get; set; }
    public required string Message { get; set; }
}

public class SmsRequest
{
    public Guid? SmsResponseId { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Message { get; set; }
    public string? Metadata { get; set; }
    public List<string>? Actions { get; set; }
}