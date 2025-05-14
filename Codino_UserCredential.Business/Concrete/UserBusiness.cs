using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Codino_UserCredential.Business.Concrete.Interfaces;
using Codino_UserCredential.Core.Dtos;
using Codino_UserCredential.Core.Enums;
using Codino_UserCredential.Core.Functions;
using Codino_UserCredential.Core.Security.Hashing;
using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models;
using Codino_UserCredential.Repository.Repositories;
using Codino_UserCredential.Repository.Repositories.Interfaces;
using Codino.UserCredential.Core.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.IdentityModel.Tokens;
using static Codino_UserCredential.Core.Security.Hashing.HashingHelper;

namespace Codino_UserCredential.Business.Concrete;

public class UserBusiness : IUserBusiness
{
    private readonly IUserRepository userRepository;
    private readonly IStringLocalizer localizer;
    private readonly IUserLoginRequestRepository userLoginRequestRepository;
    private readonly IConfiguration configuration;
    private readonly IUnitOfWork<CodinoDbContext> unitOfWork;
    
    public UserBusiness(IServiceProvider serviceProvider)
    {
        localizer = serviceProvider.GetService<IStringLocalizer<UserBusiness>>();
        userLoginRequestRepository = serviceProvider.GetService<IUserLoginRequestRepository>();
        userRepository = serviceProvider.GetService<IUserRepository>();
        configuration = serviceProvider.GetService<IConfiguration>();
        unitOfWork = serviceProvider.GetService<IUnitOfWork<CodinoDbContext>>();
    }
    
    public LoginResponse Login(LoginRequest request)
    {
        var response = new LoginResponse();
        if (!CheckValidChannel(request.Channel))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = localizer.GetString("ChannelIsOutOfRange");
            return response; 
        }

        string pass = CalculateHashForNetCore(request.Password, HashingHelper.HASH_TYPES.SHA512);
        var user = userRepository.GetQuery(u => u.Email == request.Email && u.StatusId == Status.Valid)
            .OrderByDescending(x => x.id).FirstOrDefault();

        if (user is not null && !LoadTest.IsLoadTestMode())
        {
            var loginDelayMinutes = GetIntFromConfig("LoginSettings:LoginDelayMinutes", 3);
            var maxLoginAttempts = GetIntFromConfig("LoginSettings:LoginDelayCount", 3);
        
            var otpDelay = DateTime.UtcNow.AddMinutes(-loginDelayMinutes);
        
            var recentFailedAttempts = userLoginRequestRepository.GetQuery(t =>
                t.UserId == user.id &&
                t.LoginStatus == LoginStatuses.InvalidEmailOrPassword &&
                t.creaDateTime > otpDelay
            ).Count();
            
            if (recentFailedAttempts >= maxLoginAttempts)
            {
                ClearSession(user.id); 
                return CreateErrorResponse(ResponseCode.Error, "TooManyLoginAttempts");
            }
        } 
        
        if (user is null || user.Password != CalculateHash(pass, HASH_TYPES.SHA512))
        {
            if (user is not null)
            {
                var loginRequest = new UserLoginRequest
                {
                    UserId = user.id,
                    Email = request.Email,
                    Hash = "-", 
                    Salt = "-", 
                    Cellphone = "-", 
                    Otp = "-", 
                    LoginStatus = LoginStatuses.InvalidEmailOrPassword,
                    creaDateTime = DateTime.UtcNow,
                    StatusId = Status.Valid,
                    CreaUserId = 0
                };

                
                userLoginRequestRepository.Insert(loginRequest);
                    unitOfWork.SaveChanges();
            }
            
            return CreateErrorResponse(ResponseCode.Error, "InvalidEmailOrPassword");
        }
        
        string token = GenerateJwtToken(user);
        
        var successLoginRequest = new UserLoginRequest
        {
            UserId = user.id,
            Email = request.Email,
            Hash = user.Password ?? "-", 
            Salt = "-",                 
            Cellphone = user.CellPhone ?? "-",
            Otp = "-",                
            LoginStatus = LoginStatuses.Verified,
            CreaDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            creaDateTime = DateTime.UtcNow,
            StatusId = Status.Valid,
            CreaUserId = user.id
        };
        
        userLoginRequestRepository.Insert(successLoginRequest);
        unitOfWork.SaveChanges();
        
        // Response bilgilerini doldur
        response.Code = (int)ResponseCode.Success;
        response.Message = localizer.GetString("LoginSuccessful");
        response.Id = user.id.ToString();
        response.Username = $"{user.Name} {user.Surname}";
        response.Token = token;
        
        // TODO: Diğer kullanıcı bilgilerini doldur
        // response.Level = user.Level;
        // response.Xp = user.Xp;
        // response.XpForNextLevel = CalculateXpForNextLevel(user.Level);
        // response.SelectedAvatarUrl = user.AvatarImage;
        
        return response;
    }

    public UserRegisterResponse Register(UserRegisterRequest request)
    {
        var response = new UserRegisterResponse();

        var userRegisterRequestDto = new UserRegisterRequestDto()
        {
            Name = request.Name,
            Surname = request.Surname,
            Birthdate = request.BirthDate,
        };

        var requestValidation = RegisterRequestValidation(userRegisterRequestDto);
        if (requestValidation.Code != (int)ResponseCode.Success)
        {
            response.Code = requestValidation.Code;
            response.Message = requestValidation.Message;
            return response;
        }

        if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = localizer.GetString("EmailFieldMissingOrIncorrect");
            return response;
        }

        var existingUser = userRepository.GetQuery(u => u.Email == request.Email && u.StatusId == Status.Valid).FirstOrDefault();
        if (existingUser != null)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = localizer.GetString("EmailAlreadyExists");
            return response;
        }

        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = localizer.GetString("PasswordTooShort");
            return response;
        }

        if (request.Password != request.RePassword)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = localizer.GetString("PasswordsDoNotMatch");
            return response;
        }

        if (!request.AllowKVKK)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = localizer.GetString("KVKKApprovalRequired");
            return response;
        }

        string passwordHash = CalculateHashForNetCore(request.Password, HASH_TYPES.SHA512);
        passwordHash = CalculateHash(passwordHash, HASH_TYPES.SHA512);

        
        int userTypeId = GetIntFromConfig("UserSettings:DefaultUserTypeId", 1);
        int userStatusId = GetIntFromConfig("UserSettings:DefaultUserStatusId", 1);
        
        string tckn = !string.IsNullOrWhiteSpace(request.TCKN) 
            ? request.TCKN 
            : GetStringFromConfig("UserSettings:DefaultTCKN", "00000000000");
        
        short registerChannel = request.IsChannelFromMobil 
            ? (short)RegisterChannel.Mobil 
            : (short)RegisterChannel.Web;

        var newUser = new User
        {
            Name = request.Name,
            Surname = request.Surname,
            Email = request.Email,
            CellPhone = request.Cellphone,
            Password = passwordHash,
            UserTypeId = userTypeId,
            UserStatusId = userStatusId,
            BirthDate = request.BirthDate.HasValue
                ? DateTime.SpecifyKind(request.BirthDate.Value, DateTimeKind.Utc)
                : null,
            AllowKvkk = request.AllowKVKK,
            EmailVerified = false,  
            StatusId = Status.Valid,
            creaDateTime = DateTime.UtcNow,
            CreaUserId = 0,  
            TCKN = tckn,
            LastPassword = passwordHash, 
            LastPasswordChangeDate = DateTime.UtcNow,
            hasRoboticModels = false,
            ModifDate = DateTime.UtcNow  
        };

        userRepository.Insert(newUser);
        unitOfWork.SaveChanges();

        var loginRequest = new UserLoginRequest
        {
            UserId = newUser.id,
            Email = newUser.Email,
            Hash = passwordHash,
            Salt = "-", 
            Cellphone = newUser.CellPhone ?? "-",
            Otp = "-", 
            LoginStatus = LoginStatuses.Verified,
            CreaDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            creaDateTime = DateTime.UtcNow,
            StatusId = Status.Valid,
            CreaUserId = newUser.id  
        };

        userLoginRequestRepository.Insert(loginRequest);
        unitOfWork.SaveChanges();

        int otpValidMinutes = GetIntFromConfig("UserSettings:OtpValidMinutes", 10);

        response.Code = (int)ResponseCode.Success;
        response.Message = localizer.GetString("RegisterSuccessful");
        response.Cellphone = request.Cellphone;
        response.Hash = passwordHash;
        response.OtpValidTime = DateTime.UtcNow.AddMinutes(otpValidMinutes).ToString("yyyy-MM-dd HH:mm:ss");

        return response;
    }    
    
    private string GetStringFromConfig(string key, string defaultValue)
    {
        var value = configuration[key];
        return string.IsNullOrEmpty(value) ? defaultValue : value;
    }

    public ApiResponse SetAvatar(SetAvatarRequest request)
    {
        var response = new ApiResponse();

        var user = userRepository.GetQuery(u => u.id == request.UserId && u.StatusId == Status.Valid).FirstOrDefault();
        if (user == null)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = localizer.GetString("UserNotFound");
            return response;
        }

        user.AvatarImage = request.AvatarUrl;
        userRepository.Update(user);
        unitOfWork.SaveChanges();

        response.Code = (int)ResponseCode.Success;
        response.Message = localizer.GetString("AvatarUpdated");

        return response;
    }

    public ApiResponse GetAvatar(int userId)
    {
        var response = new GetAvatarResponse();

        var user = userRepository.GetQuery(u => u.id == userId && u.StatusId == Status.Valid).FirstOrDefault();
        if (user == null)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = localizer.GetString("UserNotFound");
            response.AvatarUrl = ""; // boş döndür
            return response;
        }

        response.Code = (int)ResponseCode.Success;
        response.Message = localizer.GetString("AvatarRetrieved");
        response.AvatarUrl = string.IsNullOrWhiteSpace(user.AvatarImage) 
            ? "https://cdn.codino.com/default-avatar.png" 
            : user.AvatarImage;

        return response;    }

    public ApiResponse RegisterRequestValidation(UserRegisterRequestDto registerRequest)
    {
        var response = new ApiResponse();
        int NameAndSurnameMaxLenght = 50;
        
        if (string.IsNullOrWhiteSpace(registerRequest.Name) || registerRequest.Name.Length > NameAndSurnameMaxLenght)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = localizer.GetString("FirstNameFieldMissingInCorrect");
            return response;
        }
        
        if (string.IsNullOrWhiteSpace(registerRequest.Surname) || registerRequest.Surname.Length > NameAndSurnameMaxLenght)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = localizer.GetString("LastNameFieldMissingOrIncorrect");
            return response;
        }

        if (registerRequest.Birthdate.HasValue)
        {
            var minAge = GetIntFromConfig("RegisterSettings:MinimumAge", 13);
            if (DateTime.Today.Year - registerRequest.Birthdate.Value.Year < minAge)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = localizer.GetString("UserTooYoung");
                return response;
            }
        }

        response.Code = (int)ResponseCode.Success;
        response.Message = localizer.GetString("Success");

        return response;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private int GetIntFromConfig(string key, int defaultValue)
    {
        var value = configuration[key];
        return int.TryParse(value, out var result) ? result : defaultValue;
    }
    
    private void ClearSession(int userId)
    {
        var failedLoginAttempts = userLoginRequestRepository.GetQuery(x => x.UserId == userId && x.LoginStatus == LoginStatuses.InvalidEmailOrPassword).ToList();

        if (failedLoginAttempts.Any())
        {
            foreach (var session in failedLoginAttempts)
            {
                session.LoginStatus = LoginStatuses.SessionCleared; 
                userLoginRequestRepository.Update(session);
            }
            unitOfWork.SaveChanges();
        }
    }

    private LoginResponse CreateErrorResponse(ResponseCode code, string messageKey)
    {
        return new LoginResponse
        {
            Code = (int)code,
            Message = localizer.GetString(messageKey)
        };
    }

    private bool CheckValidChannel(short? channel)
    {
        return channel.HasValue && Enum.IsDefined(typeof(RegisterChannel), (int)channel);
    }
    
    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
    
        string role = DetermineUserRole(user.UserTypeId);
    
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.Name} {user.Surname}"),
                new Claim("UserTypeId", user.UserTypeId.ToString()),
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddHours(GetIntFromConfig("Jwt:ExpiryHours", 1)),
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };
    
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    private string DetermineUserRole(int userTypeId)
    {
        switch (userTypeId)
        {
            case 1:
                return "User";
            case 2:
                return "Admin";
            default:
                return "User"; 
        }
    }
}
