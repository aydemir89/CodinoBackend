using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using QRCoder;

namespace Codino_UserCredential.Business.Services;

public class QrCodeGenerationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<QrCodeGenerationService> _logger;
    private readonly string _qrCodeStoragePath;

    public QrCodeGenerationService(IConfiguration configuration,
        ILogger<QrCodeGenerationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _qrCodeStoragePath = "wwwroot/qrcodes";
        if (!Directory.Exists(_qrCodeStoragePath))
        {
            Directory.CreateDirectory(_qrCodeStoragePath);
        }
    }
    
    public async Task<string> GenerateUniqueActivationCodeAsync(
        string prefix = "", 
        Func<string, Task<bool>> isCodeUsedFunc = null)
    {
        string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        Random random = new Random();
        int maxAttempts = 10; 
        
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            char[] code = new char[8];
            for (int i = 0; i < 8; i++)
            {
                code[i] = chars[random.Next(chars.Length)];
            }
            
            string activationCode = prefix + new string(code);
            
            if (isCodeUsedFunc != null)
            {
                bool isUsed = await isCodeUsedFunc(activationCode);
                if (!isUsed)
                {
                    return activationCode;
                }
            }
            else
            {
                return activationCode;
            }
        }
        
        string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        return prefix + timestamp;
    }
    public async Task<string> GenerateQrCodeImageAsync(string content, string fileName)
    {
        try
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
                PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
                byte[] qrCodeImage = qrCode.GetGraphic(10);
                
                string filePath = Path.Combine(_qrCodeStoragePath, $"{fileName}.png");
                await File.WriteAllBytesAsync(filePath, qrCodeImage);
                
                return $"/qrcodes/{fileName}.png";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "QR code generation failed for content: {Content}", content);
            throw;
        }
    }

}