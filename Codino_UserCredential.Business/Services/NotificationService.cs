using Microsoft.Extensions.Logging;

namespace Codino_UserCredential.Business.Services;

public class NotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }
    
    public async Task SendToyActivationNotificationAsync(int userId, string toyName, string additionalMessage = null)
    {
        // Gerçek bir bildirim servisi entegrasyonu burada olacak
        
        string message = $"You have successfully activated your {toyName}!";
        if (!string.IsNullOrEmpty(additionalMessage))
        {
            message += $" {additionalMessage}";
        }
        
        _logger.LogInformation("Notification sent to user {UserId}: {Message}", userId, message);
        
        // Bildirim gönderme işlemi burada
        await Task.CompletedTask;
    } 
}