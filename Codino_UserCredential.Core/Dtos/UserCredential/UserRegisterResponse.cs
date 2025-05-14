using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos;

public class UserRegisterResponse : ApiResponse
{
    public string Cellphone { get; set; }
    public string Hash { get; set; }
    public string OtpValidTime { get; set; }
    
}