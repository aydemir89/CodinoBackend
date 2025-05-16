using System.ComponentModel.DataAnnotations.Schema;
using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Repository.Models.Content;

[Table("UserToy", Schema = "content")]
public class UserToy : StandartModel<int>
{
    public int UserId { get; set; }
    public int ToyId { get; set; }
    public DateTime UnlockDate { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
    
    [ForeignKey("ToyId")]
    public virtual Toy Toy { get; set; }
}