using System.Security.Cryptography;
using System.Text;

namespace Directfn.Custody.ApiFramework.Passwords
{
    public sealed class TripleDesLegacyPasswordService : ILegacyPasswordService
    {
        private const string LegacyKey = "sblw-3hn8-sqoy19";

        public bool VerifyLegacyPassword(string providedPassword, string legacyEncryptedPassword)
        {
            if (string.IsNullOrWhiteSpace(providedPassword) || string.IsNullOrWhiteSpace(legacyEncryptedPassword))
            {
                return false;
            }

            string encryptedProvidedPassword = EncryptLegacy(providedPassword);

            return string.Equals(encryptedProvidedPassword, legacyEncryptedPassword, StringComparison.Ordinal);
        }

        private static string EncryptLegacy(string input)
        {
            byte[] inputArray = Encoding.UTF8.GetBytes(input);

            using TripleDES tripleDes = TripleDES.Create();
            tripleDes.Key = Encoding.UTF8.GetBytes(LegacyKey);
            tripleDes.Mode = CipherMode.ECB;
            tripleDes.Padding = PaddingMode.PKCS7;

            using ICryptoTransform encryptor = tripleDes.CreateEncryptor();

            byte[] resultArray = encryptor.TransformFinalBlock(inputArray, 0, inputArray.Length);

            return Convert.ToBase64String(resultArray);
        }
        public string EncryptLegacyPassword(string password)
        {
            return EncryptLegacy(password);
        }
    }
}
