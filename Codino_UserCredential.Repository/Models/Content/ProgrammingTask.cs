using System.ComponentModel.DataAnnotations.Schema;

namespace Codino_UserCredential.Repository.Models.Content;

[Table("Task", Schema = "content")]
public class ProgrammingTask : StandartModel<int>
{
    public int ToyId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string ExpectedPattern { get; set; }
    
    [ForeignKey("ToyId")]
    public virtual Toy Toy { get; set; }
}