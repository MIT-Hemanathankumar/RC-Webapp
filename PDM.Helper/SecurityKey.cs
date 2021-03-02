using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace PDM.Helper
{
    public class SecurityKey
    {
        static string strPasswordPhrase = "P@ssPDM2020";// can be any string
        static string strSaltKey = "s@1tPDMValue";// can be any string
        static string strHashMethod = "MD5";// can be "MD5"SHA1
        static int iPassword = 2;// can be any number
        static string strInitialVector = "@1B2c3D4e5F6g7H8";// must be 16 bytes
        static int iPasswordKeySize = 256;// can be 192 or 128

        /// <summary>
        /// To Encrypt Hash Password
        /// </summary>
        /// <param name="passwordText">Text to be encrypted</param>
        /// <returns>Encrypted Text</returns>
        public static string EncryptHashPassword(string passwordText)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(strInitialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(strSaltKey);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(passwordText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(strPasswordPhrase, saltValueBytes, strHashMethod, iPassword);
            byte[] keyBytes = password.GetBytes(iPasswordKeySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] cipherTextBytes = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            string cipherText = Convert.ToBase64String(cipherTextBytes);
            return cipherText;
        }

        /// <summary>
        /// Decrypt the Hash Password
        /// </summary>
        /// <param name="passwordText">Text to be decrypted</param>
        /// <returns>Decrypted Text</returns>
        public static string DecryptHashPassword(string passwordText)
        {

            byte[] initVectorBytes = Encoding.ASCII.GetBytes(strInitialVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(strSaltKey);
            byte[] cipherTextBytes = Convert.FromBase64String(passwordText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(strPasswordPhrase, saltValueBytes, strHashMethod, iPassword);
            byte[] keyBytes = password.GetBytes(iPasswordKeySize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(cipherTextBytes);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            string plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            return plainText;
        }

        /// <summary>
        /// Encrypt the Password
        /// </summary>
        /// <param name="passwordText">Text to be encrypted</param>
        /// <returns>Encrypted Text</returns>
        public static string EncryptPassword(string passwordText)
        {
            int iKeyCount;
            string encryptText = string.Empty;
            char[] chCharSet = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
                       'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 
                       '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            char[] chPasswordArray = passwordText.ToCharArray();
            for (int iCounter = 0; iCounter < chPasswordArray.Length; iCounter++)
            {
                for (int Counterj = 0; Counterj < chCharSet.Length; Counterj++)
                {
                    if (chPasswordArray[iCounter] == chCharSet[Counterj])
                    {
                        iKeyCount = Counterj + 3;
                        if (iKeyCount > (chCharSet.Length - 1))
                        {
                            iKeyCount = iKeyCount - chCharSet.Length;
                        }
                        encryptText = encryptText + chCharSet[iKeyCount];
                    }
                }
            }
            return encryptText.Trim();
        }

    }
}
