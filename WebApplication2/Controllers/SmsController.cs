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
        // Check database to see if there is any conflict with Actions on the same phone number
        var existingSms = _notificationDbContext.SmsNotifications.Where(x => x.PhoneNumber == request.PhoneNumber && x.Status == SmsNotificationStatus.Active)
                                                .Include(x => x.Actions);

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
}

public class SmsResponse
{
    public Guid Id { get; set; }
    public required string Message { get; set; }
}

public class SmsRequest
{
    public required string PhoneNumber { get; set; }
    public required string Message { get; set; }
    public string? Metadata { get; set; }
    public List<string>? Actions { get; set; }
}