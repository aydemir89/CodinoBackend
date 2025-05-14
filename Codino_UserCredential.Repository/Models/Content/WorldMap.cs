using System.ComponentModel.DataAnnotations.Schema;

namespace Codino_UserCredential.Repository.Models.Content;

[Table("WorldMap", Schema = "content")]
public class WorldMap : StandartModel<int>
{
    public string Name { get; set; }
    public string BackgroundImageUrl { get; set; }
}