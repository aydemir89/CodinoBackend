namespace Codino_UserCredential.Repository.Utils;

[AttributeUsage(AttributeTargets.Class)]
public class ContextAttribute: Attribute
{
    public Type DummyTable { get; set; }
    //public string Schema { get; set; } = "dbo";
}