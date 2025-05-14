using System.Text;
using Newtonsoft.Json.Linq;

namespace Codino_UserCredential.Core.Security.Hashing;
using System.Security.Cryptography;

public static class HashingHelper
{
    public enum HASH_TYPES
    {
        SHA1 = 1,
        SHA256,
        SHA512
    }

    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using HMACSHA512 hMACSHA = new HMACSHA512();
        passwordSalt = hMACSHA.Key;
        passwordHash = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public static string BytesToString(this byte[] byteArray, Encoding encoding = null)
    {
        if (byteArray == null || byteArray.Length == 0)
            return string.Empty;
        encoding ??= Encoding.UTF8;

        return encoding.GetString(byteArray);
    }
    
    public static string CalculateHash(string input)
    {
        SHA512 sha512 = SHA512.Create();
        byte[] bytes = Encoding.UTF8.GetBytes(input);
        byte[] array = sha512.ComputeHash(bytes);
        sha512.Dispose();
        return BytesToString(array);
        
    }

    public static string CalculateHashForNetCore(string inputData, HASH_TYPES hashTypes)
    {
        string hash = "";
        if (hashTypes == HASH_TYPES.SHA1)
        {
            SHA1 sha1 = SHA1.Create();
            byte[] bytes = CodePagesEncodingProvider.Instance.GetEncoding("ISO-8859-9").GetBytes(inputData);
            byte[] hashingBytes = sha1.ComputeHash(bytes);

            hash = Convert.ToBase64String(bytes);
        }else if (hashTypes == HASH_TYPES.SHA256)
        {
            SHA256 sha256 = SHA256.Create();
            byte[] bytes = CodePagesEncodingProvider.Instance.GetEncoding("ISO-8859-9").GetBytes(inputData);
            byte[] hashingBytes = sha256.ComputeHash(bytes);

            hash = Convert.ToBase64String(bytes);
        }else if (hashTypes == HASH_TYPES.SHA512)
        {
            SHA512 sha512 = SHA512.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputData);
            byte[] hashingBytes = sha512.ComputeHash(bytes);

            hash = Convert.ToBase64String(bytes);
        }

        return hash;
    }

    public static string CalculateHash(string inputData, HASH_TYPES hashTypes)
    {
        string result = "";
        switch (hashTypes)
        {
            case HASH_TYPES.SHA1:
                byte[] bytes3 = CodePagesEncodingProvider.Instance.GetEncoding("ISO-8859-9").GetBytes(inputData);
                SHA1 sha3 = SHA1.Create();
                byte[] inArray3 = sha3.ComputeHash(bytes3);
                result = Convert.ToBase64String(inArray3);
                break;
            case HASH_TYPES.SHA256:
                byte[] bytes2 = CodePagesEncodingProvider.Instance.GetEncoding("ISO-8859-9").GetBytes(inputData);
                SHA256 sha2 = SHA256.Create();
                byte[] inArray2 = sha2.ComputeHash(bytes2);
                result = Convert.ToBase64String(inArray2);
                break;
            case HASH_TYPES.SHA512:
                byte[] bytes = CodePagesEncodingProvider.Instance.GetEncoding("ISO-8859-9").GetBytes(inputData);
                SHA512 sha = SHA512.Create();
                byte[] inArray = sha.ComputeHash(bytes);
                result = Convert.ToBase64String(inArray);
                break;
        }

        return result;
    }
}