using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos.Content.Response;

public class GetActivationCodesResponse : ApiResponse
{
    public List<ToyActivationCodeResponse> ActivationCodes { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}