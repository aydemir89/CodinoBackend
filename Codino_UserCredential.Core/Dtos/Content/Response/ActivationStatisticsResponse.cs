using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content.Response;

public class ActivationStatisticsResponse : ApiResponse
{
    public int TotalToys { get; set; }
    public int TotalActivationCodes { get; set; }
    public int TotalActivatedCodes { get; set; }
    
    public DailyStatisticsDto[] DailyStatistics { get; set; } = Array.Empty<DailyStatisticsDto>();
    public MonthlyStatisticsDto[] MonthlyStatistics { get; set; } = Array.Empty<MonthlyStatisticsDto>();
    
    public List<ToyActivationStatsDto> ToyStats { get; set; } = new();
}

public class DailyStatisticsDto
{
    public DateTime Date { get; set; }
    public int ActivationCount { get; set; }
    public int UniqueUserCount { get; set; }
}

public class MonthlyStatisticsDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public int ActivationCount { get; set; }
    public int UniqueUserCount { get; set; }
}

public class ToyActivationStatsDto
{
    public int ToyId { get; set; }
    public string ToyName { get; set; }
    public int TotalActivationCodes { get; set; }
    public int ActivatedCodesCount { get; set; }
    public double ActivationRate { get; set; } 
}