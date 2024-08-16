using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace WebApplication2.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WebhookController : ControllerBase
{
    private readonly NotificationDbContext _notificationDbContext;
    public WebhookController(NotificationDbContext notificationDbContext)
    {
        _notificationDbContext = notificationDbContext;
    }

    [HttpPost]
    public async Task<string> ProcessWebhookAsync([FromBody] SmsWebhook request)
    {

        var smsNotifications = await _notificationDbContext.SmsNotifications.Include(x => x.Actions).Where(x => x.PhoneNumber == request.From && x.Status == SmsNotificationStatus.Active).ToListAsync();

        if (!smsNotifications.Any())
        {
            return "<Response>There are no actions that can be taken at this time</Response>";
        }

        var actions = smsNotifications.SelectMany(x => x.Actions ?? []).ToList();
        if (!actions.Any(y => y.ActionText.ToLower() == request.Body?.ToLower()))
        {
            var result = new StringBuilder();
            result.Append("<Response>");
            result.Append("<Message>");
            result.Append("I'm sorry, I did not understand that. Available actions: ");
            result.Append(string.Join(", ", actions.Select(x => x.ActionText)));
            result.Append("</Message>");
            result.Append("</Response>");
            return result.ToString();
        }

        var action = actions.First(x => x.ActionText.ToLower() == request.Body?.ToLower());
        var smsResponse = new SmsResponses
        {
            Id = Guid.NewGuid(),
            SmsNotificationId = smsNotifications.First().Id,
            SmsNotificationActionId = action.Id,
            TwilioPhoneNumber = request.To,
            UserPhoneNumber = request.From
        };
        _notificationDbContext.SmsResponses.Add(smsResponse);
        await _notificationDbContext.SaveChangesAsync();

        // Send business Event to Consumer Informing Them Of The Action

        return "<Response></Response>";
    }
}

public class SmsWebhook
{
    public string? Body { get; set; }
    public string? SmsMessageSid { get; set; }
    public string? SmsSid { get; set; }
    public string? AccountSid { get; set; }
    public string? MessageSid { get; set; }
    public required string From { get; set; }
    public required string To { get; set; }
}