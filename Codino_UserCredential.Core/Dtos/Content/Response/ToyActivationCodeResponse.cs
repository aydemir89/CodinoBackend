using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content.Response;

public class ToyActivationCodeResponse : ApiResponse
{
    public int Id { get; set; }
    public int ToyId { get; set; }
    public string ToyName { get; set; }
    public string ToyImageUrl { get; set; }
    public string ActivationCode { get; set; }
    public string QrCodeImageUrl { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public bool IsActive { get; set; }
    public bool IsActivated { get; set; }
    public int? ActivatedByUserId { get; set; }
    public string ActivatedByUserName { get; set; }
    public DateTime? ActivationDate { get; set; }
}