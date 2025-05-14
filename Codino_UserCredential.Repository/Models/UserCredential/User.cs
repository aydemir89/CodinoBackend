using System.ComponentModel.DataAnnotations.Schema;
using Codino_UserCredential.Repository.Models;

namespace Codino_UserCredential.Repository.Repositories;
[Table("User", Schema = "user")]
public class User : StandartModel<int>
{
    public int UserTypeId { get; set; }
    public int UserStatusId { get; set; }
    public string Email { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Password { get; set; }
    public string TCKN { get; set; }
    public string LastPassword { get; set; }
    public DateTime? LastPasswordChangeDate { get; set; }
    public string CellPhone { get; set; }
    public bool EmailVerified { get; set; }
    public bool AllowSms { get; set; }
    public bool AllowEmail { get; set; }
    public bool AllowKvkk { get; set; }
    public DateTime? BirthDate { get; set; }
    public bool hasRoboticModels { get; set; }
    public string? AvatarImage { get; set; }
}