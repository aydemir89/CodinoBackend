namespace Codino_UserCredential.Core.Dtos.Content.Request;

public class GetActivationCodesRequest
{
    public int? ToyId { get; set; }
    public bool IncludeActivated { get; set; } = false;
    public bool IncludeExpired { get; set; } = false;
    public DateTime? CreatedAfter { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}