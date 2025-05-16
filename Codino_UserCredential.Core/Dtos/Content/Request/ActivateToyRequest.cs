namespace Codino_UserCredential.Core.Dtos.Content.Request;


public class ActivateToyRequest
{
    public string ActivationCode { get; set; }
    public int UserId { get; set; }
    public string DeviceId { get; set; } // Güvenlik için opsiyonel
}