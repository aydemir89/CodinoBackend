using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content.Response;

public class ToyActivationSummaryResponse : ApiResponse
{
    public int ToyId { get; set; }
    public string ToyName { get; set; }
    
    public int TotalActivationCodes { get; set; }
    public int ActiveCodesCount { get; set; }
    public int ActivatedCodesCount { get; set; }
    public int ExpiredCodesCount { get; set; }
    
    public int ActivationsLast24Hours { get; set; }
    public int ActivationsLast7Days { get; set; }
    public int ActivationsLast30Days { get; set; }
    
    public List<RecentActivationDto> RecentActivations { get; set; } = new();
}

public class RecentActivationDto
{
    public int Id { get; set; }
    public string ActivationCode { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; }
    public DateTime ActivationDate { get; set; }
    public string? Location { get; set; } 
}
