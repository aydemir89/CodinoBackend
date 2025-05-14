namespace Codino_UserCredential.Repository.Repositories.Interfaces;

public interface IApiResponse
{
    int Code { get; }
    string Message { get; }
}