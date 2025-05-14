using System.ComponentModel.DataAnnotations.Schema;

namespace Codino_UserCredential.Repository.Models.Content;

[Table("Toy", Schema = "content")]
public class Toy : StandartModel<int>
{
    public string Name { get; set; }
    public int BiomeId { get; set; }
    public string IconImageUrl { get; set; }
    public string Description { get; set; }
    
    [ForeignKey("BiomeId")]
    public virtual Biome Biome { get; set; }
}