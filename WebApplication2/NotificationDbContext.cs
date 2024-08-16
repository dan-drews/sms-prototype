using Microsoft.EntityFrameworkCore;

namespace WebApplication2;

public class NotificationDbContext : DbContext
{

    public DbSet<SmsNotification> SmsNotifications { get; set; }
    public DbSet<SmsNotificationAction> SmsNotificationActions { get; set; }
    public DbSet<SmsResponses> SmsResponses { get; set; }

    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
    {
    }

}

public class SmsResponses
{
    public Guid Id { get; set; }
    public Guid SmsNotificationId { get; set; }
    public Guid SmsNotificationActionId { get; set; }
    public required string TwilioPhoneNumber { get; set; }
    public required string UserPhoneNumber { get; set; }

    public SmsNotification? SmsNotification { get; set; }
    public SmsNotificationAction? SmsNotificationAction { get; set; }
}

public class SmsNotification
{
    public Guid Id { get; set; }
    public required string PhoneNumber { get; set; }
    public required string Message { get; set; }
    public SmsNotificationStatus Status { get; set; }

    // The purpose of metadata here is for the consumer to store the original context
    // That way they do not need to track this notification ID on their end
    // They could include a type and ID, or many IDs if they want, and if we send this
    // Along in the business event it can make processing very easy
    public string? Metadata { get; set; }

    public List<SmsNotificationAction>? Actions { get; set; }
}

public enum SmsNotificationStatus
{
    Active = 1,
    Completed = 2,
    Expired = 3,
    NoActions = 4
}

public class SmsNotificationAction
{
    public Guid Id { get; set; }
    public Guid SmsNotificationId { get; set; }
    public required string ActionText { get; set; }
}