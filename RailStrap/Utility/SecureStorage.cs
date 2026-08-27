using System.Security.Cryptography;

namespace RailStrap.Utility
{
    /// <summary>
    /// Encrypts small secrets (e.g. the user-supplied Roblox auth cookie for the friend activity
    /// panel) at rest using Windows DPAPI, scoped to the current Windows user account.
    /// </summary>
    static class SecureStorage
    {
        public static string Protect(string plaintext)
        {
            if (string.IsNullOrEmpty(plaintext))
                return "";

            byte[] plainBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);

            return Convert.ToBase64String(encryptedBytes);
        }

        public static string Unprotect(string encrypted)
        {
            if (string.IsNullOrEmpty(encrypted))
                return "";

            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encrypted);
                byte[] plainBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);

                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (Exception ex)
            {
                App.Logger.WriteException("SecureStorage::Unprotect", ex);
                return "";
            }
        }
    }
}
