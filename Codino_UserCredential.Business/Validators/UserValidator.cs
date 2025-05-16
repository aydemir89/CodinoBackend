using Codino_UserCredential.Core.Dtos;
using Codino_UserCredential.Core.Dtos.Content.Request;
using Codino_UserCredential.Core.Dtos.UserCredential;
using Codino_UserCredential.Core.Enums;
using Codino.UserCredential.Core.DTOs;
using Microsoft.Extensions.Localization;
using System.Text.RegularExpressions;
using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Business.Validators;

/// <summary>
/// Kullanıcı işlemleri için doğrulama sınıfı
/// </summary>
public class UserValidator
{
    private readonly IStringLocalizer<UserValidator> _localizer;
    
    public UserValidator(IStringLocalizer<UserValidator> localizer)
    {
        _localizer = localizer;
    }
    
    /// <summary>
    /// Giriş isteğini doğrular
    /// </summary>
    /// <param name="request">Giriş isteği</param>
    /// <returns>Doğrulama sonucu</returns>
    public ApiResponse ValidateLoginRequest(LoginRequest request)
    {
        var response = new ApiResponse
        {
            Code = (int)ResponseCode.Success,
            Message = _localizer.GetString("ValidationSuccessful")
        };
        
        // E-posta kontrolü
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("EmailRequired");
            return response;
        }
        
        // E-posta format kontrolü
        if (!IsValidEmail(request.Email))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("InvalidEmailFormat");
            return response;
        }
        
        // Şifre kontrolü
        if (string.IsNullOrWhiteSpace(request.Password))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("PasswordRequired");
            return response;
        }
        
        // Kanal kontrolü
        if (!request.Channel.HasValue)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("ChannelRequired");
            return response;
        }
        
        if (request.Channel.Value != (short)RegisterChannel.Web && request.Channel.Value != (short)RegisterChannel.Mobil)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("InvalidChannel");
            return response;
        }
        
        return response;
    }
    
    /// <summary>
    /// Kayıt isteğini doğrular
    /// </summary>
    /// <param name="request">Kayıt isteği</param>
    /// <returns>Doğrulama sonucu</returns>
    public ApiResponse ValidateRegisterRequest(UserRegisterRequest request)
    {
        var response = new ApiResponse
        {
            Code = (int)ResponseCode.Success,
            Message = _localizer.GetString("ValidationSuccessful")
        };
        
        // Ad kontrolü
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("NameRequired");
            return response;
        }
        
        if (request.Name.Length > 50)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("NameTooLong");
            return response;
        }
        
        // Soyad kontrolü
        if (string.IsNullOrWhiteSpace(request.Surname))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("SurnameRequired");
            return response;
        }
        
        if (request.Surname.Length > 50)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("SurnameTooLong");
            return response;
        }
        
        // E-posta kontrolü
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("EmailRequired");
            return response;
        }
        
        // E-posta format kontrolü
        if (!IsValidEmail(request.Email))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("InvalidEmailFormat");
            return response;
        }
        
        // Şifre kontrolü
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 8)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("PasswordTooShort");
            return response;
        }
        
        // Şifre tekrar kontrolü
        if (request.Password != request.RePassword)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("PasswordsDoNotMatch");
            return response;
        }
        
        // KVKK kontrolü
        if (!request.AllowKVKK)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("KVKKApprovalRequired");
            return response;
        }
        
        // TCKN kontrolü (eğer girilmişse)
        if (!string.IsNullOrEmpty(request.TCKN) && !IsValidTCKN(request.TCKN))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("InvalidTCKN");
            return response;
        }
        
        // Doğum tarihi kontrolü (eğer girilmişse)
        if (request.BirthDate.HasValue)
        {
            DateTime minDate = DateTime.UtcNow.AddYears(-13);
            if (request.BirthDate > minDate)
            {
                response.Code = (int)ResponseCode.Error;
                response.Message = _localizer.GetString("UserTooYoung");
                return response;
            }
        }
        
        return response;
    }
    
    /// <summary>
    /// Avatar ayarlama isteğini doğrular
    /// </summary>
    /// <param name="request">Avatar ayarlama isteği</param>
    /// <returns>Doğrulama sonucu</returns>
    public ApiResponse ValidateSetAvatarRequest(SetAvatarRequest request)
    {
        var response = new ApiResponse
        {
            Code = (int)ResponseCode.Success,
            Message = _localizer.GetString("ValidationSuccessful")
        };
        
        // Kullanıcı ID kontrolü
        if (request.UserId <= 0)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("UserIdRequired");
            return response;
        }
        
        // Avatar URL kontrolü
        if (string.IsNullOrWhiteSpace(request.AvatarUrl))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("AvatarUrlRequired");
            return response;
        }
        
        // URL formatı kontrolü
        if (!IsValidUrl(request.AvatarUrl))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("InvalidAvatarUrl");
            return response;
        }
        
        return response;
    }
    
    /// <summary>
    /// ID ile avatar ayarlama isteğini doğrular
    /// </summary>
    /// <param name="request">ID ile avatar ayarlama isteği</param>
    /// <returns>Doğrulama sonucu</returns>
    public ApiResponse ValidateSetUserAvatarRequest(SetUserAvatarRequest request)
    {
        var response = new ApiResponse
        {
            Code = (int)ResponseCode.Success,
            Message = _localizer.GetString("ValidationSuccessful")
        };
        
        // Kullanıcı ID kontrolü
        if (request.UserId <= 0)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("UserIdRequired");
            return response;
        }
        
        // Avatar ID kontrolü
        if (request.AvatarId <= 0)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("AvatarIdRequired");
            return response;
        }
        
        return response;
    }
    
    /// <summary>
    /// Kullanılabilir avatarları getirme isteğini doğrular
    /// </summary>
    /// <param name="request">Kullanılabilir avatarları getirme isteği</param>
    /// <returns>Doğrulama sonucu</returns>
    public ApiResponse ValidateGetAvailableAvatarsRequest(GetAvailableAvatarsRequest request)
    {
        var response = new ApiResponse
        {
            Code = (int)ResponseCode.Success,
            Message = _localizer.GetString("ValidationSuccessful")
        };
        
        // Kullanıcı ID kontrolü
        if (request.UserId <= 0)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("UserIdRequired");
            return response;
        }
        
        return response;
    }
    
    /// <summary>
    /// Kullanıcının oyuncaklarını getirme isteğini doğrular
    /// </summary>
    /// <param name="userId">Kullanıcı ID'si</param>
    /// <returns>Doğrulama sonucu</returns>
    public ApiResponse ValidateGetUserToysRequest(int userId)
    {
        var response = new ApiResponse
        {
            Code = (int)ResponseCode.Success,
            Message = _localizer.GetString("ValidationSuccessful")
        };
        
        // Kullanıcı ID kontrolü
        if (userId <= 0)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("UserIdRequired");
            return response;
        }
        
        return response;
    }
    
    /// <summary>
    /// E-posta formatını doğrular
    /// </summary>
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
    
    /// <summary>
    /// TCKN formatını doğrular
    /// </summary>
    private bool IsValidTCKN(string tckn)
    {
        // TCKN 11 haneli olmalı ve sadece rakamlardan oluşmalı
        if (string.IsNullOrWhiteSpace(tckn) || tckn.Length != 11 || !tckn.All(char.IsDigit))
            return false;
        
        // İlk rakam 0 olamaz
        if (tckn[0] == '0')
            return false;
        
        // TCKN algoritması kontrolü
        int[] digits = tckn.Select(c => c - '0').ToArray();
        
        // 1. 3. 5. 7. 9. hanelerin toplamının 7 katından, 2. 4. 6. 8. hanelerin toplamı çıkartıldığında, 
        // elde edilen sonucun 10'a bölümünden kalan, 10. haneyi vermelidir.
        int oddSum = digits[0] + digits[2] + digits[4] + digits[6] + digits[8];
        int evenSum = digits[1] + digits[3] + digits[5] + digits[7];
        
        int tenthDigit = (oddSum * 7 - evenSum) % 10;
        if (tenthDigit != digits[9])
            return false;
        
        // 1'den 10'a kadar olan rakamların toplamının 10'a bölümünden kalan, 11. haneyi vermelidir.
        int sum = digits.Take(10).Sum();
        int eleventhDigit = sum % 10;
        if (eleventhDigit != digits[10])
            return false;
        
        return true;
    }
    
    /// <summary>
    /// URL formatını doğrular
    /// </summary>
    private bool IsValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult) 
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}