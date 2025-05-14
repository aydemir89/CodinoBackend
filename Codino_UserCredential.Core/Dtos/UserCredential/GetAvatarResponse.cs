using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Core.Dtos;

public class GetAvatarResponse : ApiResponse
{
    public string AvatarUrl { get; set; }
}