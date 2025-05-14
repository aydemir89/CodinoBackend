namespace Codino_UserCredential.Core.Enums;

public static class LoginStatuses
{
    public const int Pending = 0;
    public const int Verified = 1;
    public const int Failed = 2;
    public const int InvalidEmailOrPassword = 3;
    public const int Logout = 4;
    public const int SessionCleared = 5;

    
    public enum LoginStatus
    {
        Pending = 0,
        Verified = 1,
        Failed = 2,
        InvalidEmailOrPassword = 3,
        Logout = 4,
        SessionCleared
    }
}