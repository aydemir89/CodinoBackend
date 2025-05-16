using System.ComponentModel.DataAnnotations.Schema;
using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Repository.Models.Content;

[Table("ToyActivationCode", Schema = "content")]
public class ToyActivationCode : StandartModel<int>
{
    public string ActivationCode { get; set; } 
    public int ToyId { get; set; }
    public bool IsActivated { get; set; } = false;
    public int? ActivatedByUserId { get; set; }
    public DateTime? ActivationDate { get; set; }
    
    [ForeignKey("ToyId")]
    public virtual Toy Toy { get; set; }
    
    [ForeignKey("ActivatedByUserId")]
    public virtual User ActivatedByUser { get; set; }
}