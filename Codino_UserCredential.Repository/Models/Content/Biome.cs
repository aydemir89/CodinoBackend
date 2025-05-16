using System.ComponentModel.DataAnnotations.Schema;

namespace Codino_UserCredential.Repository.Models.Content;

[Table("Biome", Schema = "content")]
public class Biome : StandartModel<int>
{
    public string Name { get; set; }
    
    [Column("WorldMapid")] 
    public int WorldMapId { get; set; }
    
    public string BackgroundImageUrl { get; set; }

    [ForeignKey("WorldMapId")]
    public virtual WorldMap WorldMapNavigation { get; set; } 
}