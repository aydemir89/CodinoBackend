using Codino_UserCredential.Repository.Repositories.Interfaces;

namespace Codino_UserCredential.Repository.Repositories;

public class ApiResponse : IApiResponse
{
    public int Code { get; set; } = 0;
    public string Message { get; set; } = "";
}