using Codino_UserCredential.Core.Dtos;
using Codino_UserCredential.Repository.Repositories;
using Codino.UserCredential.Core.DTOs;

namespace Codino_UserCredential.Business.Concrete.Interfaces;

public interface IUserBusiness
{
    LoginResponse Login(LoginRequest request);

    UserRegisterResponse Register(UserRegisterRequest request);

    ApiResponse SetAvatar(SetAvatarRequest request);

    ApiResponse GetAvatar(int userId);
}