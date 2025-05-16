using Codino_UserCredential.Core.Dtos;
using Codino_UserCredential.Core.Dtos.Content.Request;
using Codino_UserCredential.Core.Dtos.Content.Response;
using Codino_UserCredential.Repository.Repositories;
using Codino.UserCredential.Core.DTOs;

namespace Codino_UserCredential.API.Operations.Interfaces;

public interface IUserOperations
{
    LoginResponse Login(LoginRequest request);

    UserRegisterResponse Register(UserRegisterRequest request);

    ApiResponse SetAvatar(SetAvatarRequest request);

    ApiResponse GetAvatar(int userId);
    
    GetAvailableAvatarsResponse GetAvailableAvatars(GetAvailableAvatarsRequest request);
    ApiResponse SetUserAvatar(SetUserAvatarRequest request);
    
    Task<ToyActivationResponse> ActivateToyAsync(ActivateToyRequest request);
    Task<UserToysResponse> GetUserToysAsync(int userId);

}