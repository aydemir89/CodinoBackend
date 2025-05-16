namespace Codino_UserCredential.Core.Dtos.Content.Request;

public class GenerateActivationCodesRequest
{
    public int ToyId { get; set; }
    public int Count { get; set; } = 1;
    public string Prefix { get; set; } = ""; 
    public DateTime? ExpirationDate { get; set; }
}