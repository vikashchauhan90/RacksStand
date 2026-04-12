namespace RacksStands.Framework.Auth.Security;

public interface IDataProtectionService
{
    string Protect(string plainText);
    string Unprotect(string cipherText);
}
