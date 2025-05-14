using System.ComponentModel.DataAnnotations.Schema;
using Codino_UserCredential.Repository.Repositories;

namespace Codino_UserCredential.Repository.Models.Content;

[Table("TaskSubmission", Schema = "content")]
public class TaskSubmission : StandartModel<int>
{
    public int UserId { get; set; }
    public int TaskId { get; set; }
    public string SubmittedCode { get; set; }
    public bool IsCorrect { get; set; }
    public DateTime Timestamp { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
    
    [ForeignKey("TaskId")]
    public virtual ProgrammingTask ProgrammingTask { get; set; }
}