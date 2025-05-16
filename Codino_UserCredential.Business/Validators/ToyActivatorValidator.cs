using Codino_UserCredential.Core.Dtos;
using Codino_UserCredential.Core.Dtos.Content.Request;
using Codino_UserCredential.Core.Enums;
using Codino_UserCredential.Repository.Repositories;
using Microsoft.Extensions.Localization;

namespace Codino_UserCredential.Business.Validators;

/// <summary>
/// Oyuncak aktivasyon süreçleri için doğrulama sınıfı
/// </summary>
public class ToyActivatorValidator
{
    private readonly IStringLocalizer<ToyActivatorValidator> _localizer;
    
    public ToyActivatorValidator(IStringLocalizer<ToyActivatorValidator> localizer) 
    {
        _localizer = localizer;
    }
    
    /// <summary>
    /// Aktivasyon kodu isteğini doğrular
    /// </summary>
    /// <param name="request">Aktivasyon kodu isteği</param>
    /// <returns>Doğrulama sonucu</returns>
    public ApiResponse ValidateActivateToyRequest(ActivateToyRequest request)
    {
        var response = new ApiResponse
        {
            Code = (int)ResponseCode.Success,
            Message = _localizer.GetString("ValidationSuccessful")
        };
        
        // Aktivasyon kodu boş olamaz
        if (string.IsNullOrWhiteSpace(request.ActivationCode))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("ActivationCodeRequired");
            return response;
        }
        
        // Aktivasyon kodu formatı kontrol et (sadece harf ve rakam)
        if (!IsValidActivationCode(request.ActivationCode))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("InvalidActivationCodeFormat");
            return response;
        }
        
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
    /// Aktivasyon kodu formatını doğrular (sadece harf ve rakam)
    /// </summary>
    private bool IsValidActivationCode(string code)
    {
        // Aktivasyon kodu formatı: A-Z, 0-9 (I, O, 0, 1 hariç) ve 8 karakter uzunluğunda
        if (code.Length != 8)
            return false;
        
        foreach (char c in code)
        {
            if (!((c >= 'A' && c <= 'Z' && c != 'I' && c != 'O') || 
                 (c >= '2' && c <= '9')))
            {
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Aktivasyon kodu oluşturma isteğini doğrular
    /// </summary>
    /// <param name="request">Aktivasyon kodu oluşturma isteği</param>
    /// <returns>Doğrulama sonucu</returns>
    public ApiResponse ValidateGenerateActivationCodesRequest(GenerateActivationCodesRequest request)
    {
        var response = new ApiResponse
        {
            Code = (int)ResponseCode.Success,
            Message = _localizer.GetString("ValidationSuccessful")
        };
        
        // Toy ID kontrolü
        if (request.ToyId <= 0)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("ToyIdRequired");
            return response;
        }
        
        // Kod adedi kontrolü
        if (request.Count <= 0 || request.Count > 1000)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("InvalidCodeCount");
            return response;
        }
        
        // Önek formatı kontrolü
        if (!string.IsNullOrEmpty(request.Prefix) && request.Prefix.Length > 5)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("PrefixTooLong");
            return response;
        }
        
        // Ön ek karakter kontrolü (sadece harf ve rakam)
        if (!string.IsNullOrEmpty(request.Prefix) && !IsValidPrefix(request.Prefix))
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("InvalidPrefixFormat");
            return response;
        }
        
        // Geçerlilik süresi kontrolü
        if (request.ExpirationDate.HasValue && request.ExpirationDate < DateTime.UtcNow)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("ExpirationDateCannotBeInPast");
            return response;
        }
        
        return response;
    }
    
    /// <summary>
    /// Önek formatını doğrular (sadece harf ve rakam)
    /// </summary>
    private bool IsValidPrefix(string prefix)
    {
        foreach (char c in prefix)
        {
            if (!char.IsLetterOrDigit(c))
            {
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Aktivasyon detayları isteğini doğrular
    /// </summary>
    /// <param name="activationCodeId">Aktivasyon kodu ID'si</param>
    /// <returns>Doğrulama sonucu</returns>
    public ApiResponse ValidateActivationDetailsRequest(int activationCodeId)
    {
        var response = new ApiResponse
        {
            Code = (int)ResponseCode.Success,
            Message = _localizer.GetString("ValidationSuccessful")
        };
        
        // Aktivasyon kodu ID kontrolü
        if (activationCodeId <= 0)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("ActivationCodeIdRequired");
            return response;
        }
        
        return response;
    }
    
    /// <summary>
    /// Aktivasyon özeti isteğini doğrular
    /// </summary>
    /// <param name="toyId">Oyuncak ID'si</param>
    /// <returns>Doğrulama sonucu</returns>
    public ApiResponse ValidateActivationSummaryRequest(int toyId)
    {
        var response = new ApiResponse
        {
            Code = (int)ResponseCode.Success,
            Message = _localizer.GetString("ValidationSuccessful")
        };
        
        // Toy ID kontrolü
        if (toyId <= 0)
        {
            response.Code = (int)ResponseCode.Error;
            response.Message = _localizer.GetString("ToyIdRequired");
            return response;
        }
        
        return response;
    }
}