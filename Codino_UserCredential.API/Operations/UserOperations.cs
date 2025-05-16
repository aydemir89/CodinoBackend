using Codino_UserCredential.API.Operations.Interfaces;
using Codino_UserCredential.Business.Concrete;
using Codino_UserCredential.Business.Concrete.Interfaces;
using Codino_UserCredential.Core.Dtos;
using Codino_UserCredential.Core.Dtos.Content.Request;
using Codino_UserCredential.Core.Dtos.Content.Response;
using Codino_UserCredential.Repository.Repositories;
using Codino.UserCredential.Core.DTOs;

namespace Codino_UserCredential.API.Operations;

public class UserOperations : IUserOperations
{
    private readonly IUserBusiness userBusiness;
    
    public UserOperations(IUserBusiness userBusiness)
    {
        this.userBusiness = userBusiness;
    }
    public LoginResponse Login(LoginRequest request)
    {
        return userBusiness.Login(request);
    }
    
    public UserRegisterResponse Register(UserRegisterRequest request)
    {
        return userBusiness.Register(request);
    }

    public ApiResponse SetAvatar(SetAvatarRequest request)
    {
        return userBusiness.SetAvatar(request);
    }

    public ApiResponse GetAvatar(int userId)
    {
        return userBusiness.GetAvatar(userId);
    }
    
    public GetAvailableAvatarsResponse GetAvailableAvatars(GetAvailableAvatarsRequest request)
    {
        return userBusiness.GetAvailableAvatars(request);
    }

    public ApiResponse SetUserAvatar(SetUserAvatarRequest request)
    {
        return userBusiness.SetUserAvatar(request);
    }

    public async Task<ToyActivationResponse> ActivateToyAsync(ActivateToyRequest request)
    {
        return await userBusiness.ActivateToyAsync(request);
    }
    public async Task<UserToysResponse> GetUserToysAsync(int userId)
    {
        return await userBusiness.GetUserToysAsync(userId);
    }
}