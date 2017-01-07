using System.Text;

namespace TfsStandalone.UI.Infrastructure.Security
{
    using System.Security.Cryptography;

    public class CredentialsManager
    {
        private static byte[] _entropy;
        private static byte[] _encrypted;
        private static string _username;

        public UserPass RetrieveCredentials()
        {
            var plaintext = ProtectedData.Unprotect(_encrypted, _entropy, DataProtectionScope.CurrentUser);
            return new UserPass { Username = _username, Password = Encoding.UTF8.GetString(plaintext) };
        }

        public void StoreCredentials(string username, string password)
        {
            var plaintext = Encoding.UTF8.GetBytes(password);

            // entropy is optional, but ensures encrypted data is more difficult for other applications to access
            _entropy = GenerateEntropy();
            _encrypted = ProtectedData.Protect(plaintext, _entropy, DataProtectionScope.CurrentUser);
            _username = username;
        }
        
        private byte[] GenerateEntropy()
        {
            var entropy = new byte[20];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(entropy);
            }

            return entropy;
        }
    }
}
