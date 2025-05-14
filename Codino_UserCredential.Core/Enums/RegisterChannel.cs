using System.ComponentModel.DataAnnotations;

namespace Codino_UserCredential.Core.Enums;

public enum RegisterChannel
{
    [Display(Name = "Web")]
    Web,
    [Display(Name = "Mobil")]
    Mobil,
}