using System.ComponentModel.DataAnnotations.Schema;

namespace Codino_UserCredential.Repository.Models.Content;

[Table("ToyAvatar", Schema = "content")]
public class ToyAvatar : StandartModel<int>
{
    public int ToyId { get; set; }
    public string AvatarUrl { get; set; }
    public string Name { get; set; }
    public int RequiredToyLevel { get; set; } = 1; // İleride oyuncak seviyesi için
    public int RequiredUserXp { get; set; } = 0; // İleride XP gereksinimleri için
    
    [ForeignKey("ToyId")]
    public virtual Toy Toy { get; set; }
}