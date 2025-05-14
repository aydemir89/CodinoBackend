using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Codino_UserCredential.Core.Enums;
using Codino_UserCredential.Repository.Repositories.Interfaces;

namespace Codino_UserCredential.Repository.Models;

public class StandartModel<K>: IStandartModel<K>, IBaseModel where K : struct
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public K id { get; set; }
    public Status StatusId { get; set; }
    public int CreaUserId { get; set; }
    public DateTime creaDateTime { get; set; }
    public DateTime ModifDate { get; set; }
}