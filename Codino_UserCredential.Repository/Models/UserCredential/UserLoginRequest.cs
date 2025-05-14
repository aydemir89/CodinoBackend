using System.ComponentModel.DataAnnotations.Schema;
using Codino_UserCredential.Repository.Models;

[Table("User", Schema = "user")]
public class UserLoginRequest : StandartModel<long>
{
    public int UserId { get; set; }
    public int CreaDate { get; set; }
    public string Email { get; set; }
    public string Hash { get; set; }
    public string Salt { get; set; }
    public string Cellphone { get; set; }
    public string Otp { get; set; }
    public int LoginStatus { get; set; }
}